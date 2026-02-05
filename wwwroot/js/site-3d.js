document.addEventListener('DOMContentLoaded', () => {
    // 1) Find the container
    const container = document.getElementById('canvas-container');
    if (!container) return;

    // 2) Scene Setup
    const scene = new THREE.Scene();

    // 3) Camera
    const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    camera.position.z = 30;

    // 4) Renderer
    const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setSize(window.innerWidth, window.innerHeight);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    container.appendChild(renderer.domElement);

    // Get accent color from CSS
    const computedStyle = getComputedStyle(document.documentElement);
    const accentColor = computedStyle.getPropertyValue('--accent').trim() || '#38bdf8';

    // ========== CONFIGURATION ==========
    const CONFIG = {
        particleCount: 200,
        maxMagnetParticles: 15,       // Only attract this many particles at once
        magnetRadius: 8,              // How close cursor needs to be to attract
        magnetStrength: 0.04,         // Pull strength (gentle)
        maxDragDistance: 12,          // Max distance from original position before returning
        returnSpeed: 0.02,            // How fast particles return home
        explosionForce: 1.2,          // Explosion power on click
        damping: 0.92                 // Velocity damping (more friction)
    };

    // ========== MAIN PARTICLES ==========
    const geometry = new THREE.BufferGeometry();
    const positions = new Float32Array(CONFIG.particleCount * 3);
    const colors = new Float32Array(CONFIG.particleCount * 3);
    const sizes = new Float32Array(CONFIG.particleCount);
    const velocities = [];
    const originalPositions = [];
    const particleStates = []; // Track if particle is being attracted or returning

    const baseColor = new THREE.Color(accentColor);

    for (let i = 0; i < CONFIG.particleCount; i++) {
        const x = (Math.random() - 0.5) * 80;
        const y = (Math.random() - 0.5) * 50;
        const z = (Math.random() - 0.5) * 30;

        positions[i * 3] = x;
        positions[i * 3 + 1] = y;
        positions[i * 3 + 2] = z;

        originalPositions.push({ x, y, z });
        particleStates.push({ isAttracted: false, distance: 0 });

        // Color variation
        colors[i * 3] = baseColor.r + (Math.random() - 0.5) * 0.2;
        colors[i * 3 + 1] = baseColor.g + (Math.random() - 0.5) * 0.1;
        colors[i * 3 + 2] = baseColor.b + (Math.random() - 0.5) * 0.1;

        // Random sizes
        sizes[i] = Math.random() * 0.3 + 0.15;

        // Ambient drift velocities (particles always moving slowly)
        velocities.push({
            x: (Math.random() - 0.5) * 0.015,
            y: (Math.random() - 0.5) * 0.015,
            z: (Math.random() - 0.5) * 0.01
        });
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));
    geometry.setAttribute('size', new THREE.BufferAttribute(sizes, 1));

    const material = new THREE.PointsMaterial({
        size: 0.4,
        vertexColors: true,
        transparent: true,
        opacity: 0.75,
        blending: THREE.AdditiveBlending,
        sizeAttenuation: true
    });

    const particles = new THREE.Points(geometry, material);
    scene.add(particles);

    // ========== TRAIL PARTICLES ==========
    const trailCount = 50;
    const trailGeometry = new THREE.BufferGeometry();
    const trailPositions = new Float32Array(trailCount * 3);
    const trailLifetimes = [];

    for (let i = 0; i < trailCount; i++) {
        trailPositions[i * 3] = 0;
        trailPositions[i * 3 + 1] = 0;
        trailPositions[i * 3 + 2] = -1000;
        trailLifetimes.push({ active: false, life: 0, maxLife: 25 });
    }

    trailGeometry.setAttribute('position', new THREE.BufferAttribute(trailPositions, 3));

    const trailMaterial = new THREE.PointsMaterial({
        color: new THREE.Color(accentColor),
        size: 0.2,
        transparent: true,
        opacity: 0.5,
        blending: THREE.AdditiveBlending
    });

    const trailParticles = new THREE.Points(trailGeometry, trailMaterial);
    scene.add(trailParticles);

    let trailIndex = 0;

    // ========== MOUSE TRACKING (ACCURATE) ==========
    let mouse3D = new THREE.Vector3(0, 0, 0);
    let mouseScreen = new THREE.Vector2(0, 0);
    let prevMouseScreen = new THREE.Vector2(0, 0);
    let mouseSpeed = 0;
    const raycaster = new THREE.Raycaster();
    const mousePlane = new THREE.Plane(new THREE.Vector3(0, 0, 1), 0);

    function updateMouse3D(event) {
        prevMouseScreen.copy(mouseScreen);

        mouseScreen.x = (event.clientX / window.innerWidth) * 2 - 1;
        mouseScreen.y = -(event.clientY / window.innerHeight) * 2 + 1;

        // Calculate speed
        const dx = mouseScreen.x - prevMouseScreen.x;
        const dy = mouseScreen.y - prevMouseScreen.y;
        mouseSpeed = Math.sqrt(dx * dx + dy * dy);

        // Accurate 3D position using raycaster
        raycaster.setFromCamera(mouseScreen, camera);
        raycaster.ray.intersectPlane(mousePlane, mouse3D);
    }

    document.addEventListener('mousemove', (event) => {
        updateMouse3D(event);

        // Spawn trail on fast movement
        if (mouseSpeed > 0.015) {
            spawnTrailParticle(mouse3D.x, mouse3D.y, mouse3D.z);
        }
    });

    // ========== CLICK EXPLOSION (LOCAL) ==========
    document.addEventListener('click', (event) => {
        updateMouse3D(event);
        triggerExplosion(mouse3D.x, mouse3D.y, mouse3D.z);
    });

    function triggerExplosion(originX, originY, originZ) {
        const posArr = particles.geometry.attributes.position.array;
        const explosionRadius = 15; // Only affect particles within this radius

        for (let i = 0; i < CONFIG.particleCount; i++) {
            const px = posArr[i * 3];
            const py = posArr[i * 3 + 1];
            const pz = posArr[i * 3 + 2];

            // Direction away from click point
            let dx = px - originX;
            let dy = py - originY;
            let dz = pz - originZ;
            const dist = Math.sqrt(dx * dx + dy * dy + dz * dz);

            // Only affect particles within explosion radius
            if (dist < explosionRadius && dist > 0.1) {
                // Force falls off with distance (closer = stronger)
                const falloff = 1 - (dist / explosionRadius);
                const force = CONFIG.explosionForce * falloff * falloff;

                velocities[i].x += (dx / dist) * force + (Math.random() - 0.5) * 0.3;
                velocities[i].y += (dy / dist) * force + (Math.random() - 0.5) * 0.3;
                velocities[i].z += (dz / dist) * force * 0.2;
            }
        }
    }

    // ========== TRAIL SPAWNER ==========
    function spawnTrailParticle(x, y, z) {
        const trailPos = trailGeometry.attributes.position.array;

        trailPos[trailIndex * 3] = x + (Math.random() - 0.5) * 1.5;
        trailPos[trailIndex * 3 + 1] = y + (Math.random() - 0.5) * 1.5;
        trailPos[trailIndex * 3 + 2] = z;

        trailLifetimes[trailIndex].active = true;
        trailLifetimes[trailIndex].life = trailLifetimes[trailIndex].maxLife;

        trailIndex = (trailIndex + 1) % trailCount;
        trailGeometry.attributes.position.needsUpdate = true;
    }

    // ========== ANIMATION LOOP ==========
    const animate = () => {
        requestAnimationFrame(animate);

        const posArr = particles.geometry.attributes.position.array;
        const sizeArr = particles.geometry.attributes.size.array;

        // Calculate distances to cursor for all particles
        const particleDistances = [];
        for (let i = 0; i < CONFIG.particleCount; i++) {
            const px = posArr[i * 3];
            const py = posArr[i * 3 + 1];
            const pz = posArr[i * 3 + 2];

            const dx = mouse3D.x - px;
            const dy = mouse3D.y - py;
            const dz = mouse3D.z - pz;
            const dist = Math.sqrt(dx * dx + dy * dy + dz * dz);

            particleDistances.push({ index: i, distance: dist, dx, dy, dz });
        }

        // Sort by distance and only attract the closest N particles
        particleDistances.sort((a, b) => a.distance - b.distance);
        const attractedSet = new Set();

        for (let j = 0; j < Math.min(CONFIG.maxMagnetParticles, particleDistances.length); j++) {
            const item = particleDistances[j];
            if (item.distance < CONFIG.magnetRadius) {
                attractedSet.add(item.index);
            }
        }

        // Update each particle
        for (let i = 0; i < CONFIG.particleCount; i++) {
            const px = posArr[i * 3];
            const py = posArr[i * 3 + 1];
            const pz = posArr[i * 3 + 2];

            const ox = originalPositions[i].x;
            const oy = originalPositions[i].y;
            const oz = originalPositions[i].z;

            // Distance from original position
            const distFromOrigin = Math.sqrt(
                (px - ox) ** 2 + (py - oy) ** 2 + (pz - oz) ** 2
            );

            const isAttracted = attractedSet.has(i);
            const tooFar = distFromOrigin > CONFIG.maxDragDistance;

            if (isAttracted && !tooFar) {
                // Magnet pull towards cursor
                const dx = mouse3D.x - px;
                const dy = mouse3D.y - py;
                const dz = mouse3D.z - pz;
                const dist = Math.sqrt(dx * dx + dy * dy + dz * dz) || 1;

                const force = CONFIG.magnetStrength * (1 - dist / CONFIG.magnetRadius);
                velocities[i].x += (dx / dist) * force;
                velocities[i].y += (dy / dist) * force;
                velocities[i].z += (dz / dist) * force * 0.3;

                // Glow when attracted
                sizeArr[i] = Math.min(sizeArr[i] + 0.03, 0.7);
                particleStates[i].isAttracted = true;
            } else {
                // Return to original position
                const returnX = ox - px;
                const returnY = oy - py;
                const returnZ = oz - pz;

                const returnDist = Math.sqrt(returnX ** 2 + returnY ** 2 + returnZ ** 2);
                if (returnDist > 0.1) {
                    // Smoother return - scales with distance but with lower base force
                    const returnForce = CONFIG.returnSpeed * 0.5;
                    velocities[i].x += (returnX / returnDist) * returnForce * Math.min(returnDist, 5) * 0.08;
                    velocities[i].y += (returnY / returnDist) * returnForce * Math.min(returnDist, 5) * 0.08;
                    velocities[i].z += (returnZ / returnDist) * returnForce * Math.min(returnDist, 5) * 0.08;
                }

                // Shrink back
                sizeArr[i] = Math.max(sizeArr[i] - 0.01, 0.15 + Math.random() * 0.15);
                particleStates[i].isAttracted = false;
            }

            // Apply velocity with damping
            velocities[i].x *= CONFIG.damping;
            velocities[i].y *= CONFIG.damping;
            velocities[i].z *= CONFIG.damping;

            // Maintain minimum ambient drift (keep particles always moving)
            const minDrift = 0.008;
            const currentSpeed = Math.sqrt(velocities[i].x ** 2 + velocities[i].y ** 2);
            if (currentSpeed < minDrift && !isAttracted) {
                // Add gentle random drift when moving too slowly
                velocities[i].x += (Math.random() - 0.5) * 0.01;
                velocities[i].y += (Math.random() - 0.5) * 0.01;
            }

            posArr[i * 3] += velocities[i].x;
            posArr[i * 3 + 1] += velocities[i].y;
            posArr[i * 3 + 2] += velocities[i].z;

            // Soft boundary (prevent particles from flying too far)
            const maxBound = 80;
            if (Math.abs(posArr[i * 3]) > maxBound) velocities[i].x *= -0.5;
            if (Math.abs(posArr[i * 3 + 1]) > maxBound) velocities[i].y *= -0.5;
            if (Math.abs(posArr[i * 3 + 2]) > maxBound) velocities[i].z *= -0.5;
        }

        particles.geometry.attributes.position.needsUpdate = true;
        particles.geometry.attributes.size.needsUpdate = true;

        // ========== UPDATE TRAILS ==========
        const trailPos = trailGeometry.attributes.position.array;
        let activeTrails = 0;

        for (let i = 0; i < trailCount; i++) {
            if (trailLifetimes[i].active) {
                trailLifetimes[i].life -= 1;
                if (trailLifetimes[i].life <= 0) {
                    trailLifetimes[i].active = false;
                    trailPos[i * 3 + 2] = -1000;
                } else {
                    activeTrails++;
                    trailPos[i * 3 + 2] -= 0.08;
                }
            }
        }

        trailMaterial.opacity = activeTrails > 0 ? 0.4 : 0;
        trailGeometry.attributes.position.needsUpdate = true;

        // Gentle scene rotation + mouse parallax
        scene.rotation.x += 0.0001;
        scene.rotation.y += 0.0001;

        const targetRotX = mouseScreen.y * 0.03;
        const targetRotY = mouseScreen.x * 0.03;
        scene.rotation.x += (targetRotX - scene.rotation.x) * 0.01;
        scene.rotation.y += (targetRotY - scene.rotation.y) * 0.01;

        renderer.render(scene, camera);
    };

    animate();

    // ========== RESIZE HANDLER ==========
    window.addEventListener('resize', () => {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    });

    // ========== THEME CHANGE HANDLER ==========
    const observer = new MutationObserver(() => {
        const newAccent = getComputedStyle(document.documentElement).getPropertyValue('--accent').trim();
        if (newAccent) {
            const newColor = new THREE.Color(newAccent);
            trailMaterial.color = newColor;

            const colorsArr = particles.geometry.attributes.color.array;
            for (let i = 0; i < CONFIG.particleCount; i++) {
                colorsArr[i * 3] = newColor.r + (Math.random() - 0.5) * 0.2;
                colorsArr[i * 3 + 1] = newColor.g + (Math.random() - 0.5) * 0.1;
                colorsArr[i * 3 + 2] = newColor.b + (Math.random() - 0.5) * 0.1;
            }
            particles.geometry.attributes.color.needsUpdate = true;
        }
    });

    observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });

    // ========== SECTION-BASED PARTICLE MIGRATION & BREATHING LIGHT ==========
    let currentSection = null;
    let breathingIntensity = 0;
    let breathingTarget = 0;

    // Create breathing light overlay
    const breathingLight = document.createElement('div');
    breathingLight.id = 'breathing-light';
    breathingLight.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        pointer-events: none;
        z-index: -1;
        opacity: 0;
        background: radial-gradient(
            ellipse at 50% 50%,
            rgba(56, 189, 248, 0.08) 0%,
            transparent 70%
        );
        transition: opacity 0.5s ease-out;
    `;
    document.body.appendChild(breathingLight);

    // Update breathing light color on theme change
    const updateBreathingColor = () => {
        const accent = getComputedStyle(document.documentElement).getPropertyValue('--accent').trim() || '#38bdf8';
        const rgb = new THREE.Color(accent);
        breathingLight.style.background = `
            radial-gradient(
                ellipse at 50% 50%,
                rgba(${Math.round(rgb.r * 255)}, ${Math.round(rgb.g * 255)}, ${Math.round(rgb.b * 255)}, 0.1) 0%,
                transparent 70%
            )
        `;
    };
    updateBreathingColor();

    // ========== CLUSTER CONFIGURATION ==========
    const clusterCenters = []; // Active cluster centers
    const particleClusterAssignment = []; // Which cluster each particle belongs to

    // Initialize cluster assignments
    for (let i = 0; i < CONFIG.particleCount; i++) {
        particleClusterAssignment.push(0);
    }

    // Generate initial clusters
    function generateClusters(numClusters = 4) {
        clusterCenters.length = 0;
        for (let c = 0; c < numClusters; c++) {
            clusterCenters.push({
                x: (Math.random() - 0.5) * 60,
                y: (Math.random() - 0.5) * 35,
                z: (Math.random() - 0.5) * 20,
                radius: 8 + Math.random() * 12 // Cluster spread radius
            });
        }
    }
    generateClusters(4);

    // Function to migrate particles into clusters
    function migrateParticles() {
        // Generate new cluster centers (3-5 clusters)
        const numClusters = 3 + Math.floor(Math.random() * 3);
        generateClusters(numClusters);

        for (let i = 0; i < CONFIG.particleCount; i++) {
            // Assign particle to a random cluster
            const clusterIdx = Math.floor(Math.random() * numClusters);
            particleClusterAssignment[i] = clusterIdx;

            const cluster = clusterCenters[clusterIdx];

            // New position is within cluster radius (particles gather together)
            const angle = Math.random() * Math.PI * 2;
            const radius = Math.random() * cluster.radius;
            const height = (Math.random() - 0.5) * cluster.radius * 0.6;

            const newX = cluster.x + Math.cos(angle) * radius;
            const newY = cluster.y + Math.sin(angle) * radius * 0.7;
            const newZ = cluster.z + height;

            // Update original positions (where particles vibrate around)
            originalPositions[i].x = newX;
            originalPositions[i].y = newY;
            originalPositions[i].z = newZ;

            // Smooth push towards new cluster position (very gentle for fluid motion)
            const posArr = particles.geometry.attributes.position.array;
            const dx = newX - posArr[i * 3];
            const dy = newY - posArr[i * 3 + 1];
            const dz = newZ - posArr[i * 3 + 2];

            // Much gentler push for smooth drifting (not snapping)
            velocities[i].x += dx * 0.012;
            velocities[i].y += dy * 0.012;
            velocities[i].z += dz * 0.008;
        }

        // Trigger breathing light pulse
        breathingTarget = 1;
        breathingLight.style.opacity = '1';

        // Fade out breathing light
        setTimeout(() => {
            breathingTarget = 0;
            breathingLight.style.opacity = '0';
        }, 600);
    }

    // ========== PROGRESSIVE PARTICLE VISIBILITY ==========
    let visibleParticleRatio = 0.3; // Start with 30% visible at top
    let sectionIndex = 0;
    const sectionList = Array.from(document.querySelectorAll('section[id]'));

    // Update particle visibility based on scroll/section
    function updateParticleVisibility() {
        const sizeArr = particles.geometry.attributes.size.array;
        const targetVisible = Math.floor(CONFIG.particleCount * visibleParticleRatio);

        for (let i = 0; i < CONFIG.particleCount; i++) {
            if (i < targetVisible) {
                // Visible particles - restore size
                if (sizeArr[i] < 0.15) {
                    sizeArr[i] = 0.15 + Math.random() * 0.2;
                }
            } else {
                // Hidden particles - shrink to invisible
                sizeArr[i] = Math.max(0, sizeArr[i] - 0.01);
            }
        }
        particles.geometry.attributes.size.needsUpdate = true;
    }

    // Intersection Observer for sections with rapid scroll handling
    const sections = document.querySelectorAll('section[id]');
    let pendingSection = null;
    let migrationTimeout = null;
    const migrationCooldown = 300; // Minimum ms between migrations

    const sectionObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && entry.intersectionRatio > 0.3) {
                const sectionId = entry.target.id;

                // Only trigger if section actually changed
                if (currentSection !== sectionId) {
                    currentSection = sectionId;
                    pendingSection = sectionId;

                    // Calculate section index for progressive reveal
                    const idx = sectionList.findIndex(s => s.id === sectionId);
                    if (idx !== -1) {
                        sectionIndex = idx;
                        // More particles visible as you scroll down (30% -> 100%)
                        visibleParticleRatio = 0.3 + (sectionIndex / Math.max(sectionList.length - 1, 1)) * 0.7;
                        updateParticleVisibility();
                    }

                    // Throttle migrations - if scrolling fast, cancel pending and use latest
                    if (migrationTimeout) {
                        clearTimeout(migrationTimeout);
                    }

                    migrationTimeout = setTimeout(() => {
                        // Only migrate if this is still the pending section
                        if (pendingSection === sectionId) {
                            migrateParticles();

                            // Update breathing light position based on section
                            const rect = entry.target.getBoundingClientRect();
                            const centerY = ((rect.top + rect.height / 2) / window.innerHeight) * 100;
                            breathingLight.style.background = `
                                radial-gradient(
                                    ellipse at 50% ${Math.min(Math.max(centerY, 20), 80)}%,
                                    rgba(56, 189, 248, 0.12) 0%,
                                    transparent 60%
                                )
                            `;
                            updateBreathingColor();
                        }
                        migrationTimeout = null;
                    }, migrationCooldown);
                }
            }
        });
    }, {
        threshold: [0.3, 0.5],
        rootMargin: '-10% 0px -10% 0px'
    });

    sections.forEach(section => {
        sectionObserver.observe(section);
    });

    // Initial visibility update
    updateParticleVisibility();

    // Keep particles within reasonable bounds after migration
    const enforceBounds = () => {
        for (let i = 0; i < CONFIG.particleCount; i++) {
            originalPositions[i].x = Math.max(-60, Math.min(60, originalPositions[i].x));
            originalPositions[i].y = Math.max(-35, Math.min(35, originalPositions[i].y));
            originalPositions[i].z = Math.max(-25, Math.min(25, originalPositions[i].z));
        }
    };

    // Periodically enforce bounds
    setInterval(enforceBounds, 2000);

    console.log('✨ Particle system with section-based migration initialized');
});
