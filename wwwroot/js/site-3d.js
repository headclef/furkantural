document.addEventListener('DOMContentLoaded', () => {
    // 1) Find the container (fix: ensure it exists in _Layout)
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
    renderer.setPixelRatio(window.devicePixelRatio);
    container.appendChild(renderer.domElement);

    // 5) Particles
    // Let's make it a bit denser since it's full screen
    const particleCount = 150;
    const geometry = new THREE.BufferGeometry();
    const positions = new Float32Array(particleCount * 3);
    const velocities = [];

    for (let i = 0; i < particleCount; i++) {
        // Spread is wider for full screen
        positions[i * 3] = (Math.random() - 0.5) * 80;
        positions[i * 3 + 1] = (Math.random() - 0.5) * 50;
        positions[i * 3 + 2] = (Math.random() - 0.5) * 50;

        velocities.push({
            x: (Math.random() - 0.5) * 0.02, // Slower, more "ambient"
            y: (Math.random() - 0.5) * 0.02,
            z: (Math.random() - 0.5) * 0.02
        });
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));

    // Get accent color from CSS variable if possible, else default to blue
    // Creating a dummy element to read variable
    const computedStyle = getComputedStyle(document.documentElement);
    const accentColor = computedStyle.getPropertyValue('--accent').trim() || '#38bdf8';

    const material = new THREE.PointsMaterial({
        color: new THREE.Color(accentColor),
        size: 0.25,
        transparent: true,
        opacity: 0.6
    });

    const particles = new THREE.Points(geometry, material);
    scene.add(particles);

    // 6) Mouse Interaction
    let mouseX = 0;
    let mouseY = 0;

    document.addEventListener('mousemove', (event) => {
        mouseX = (event.clientX / window.innerWidth) * 2 - 1;
        mouseY = -(event.clientY / window.innerHeight) * 2 + 1;
    });

    // 7) Animation Loop
    const animate = () => {
        requestAnimationFrame(animate);

        const posArr = particles.geometry.attributes.position.array;

        for (let i = 0; i < particleCount; i++) {
            // Move
            posArr[i * 3] += velocities[i].x;
            posArr[i * 3 + 1] += velocities[i].y;
            posArr[i * 3 + 2] += velocities[i].z;

            // Bounce / Wrap
            // If they go too far, wrap them around to the other side for seamless feeling
            if (posArr[i * 3] > 60) posArr[i * 3] = -60;
            if (posArr[i * 3] < -60) posArr[i * 3] = 60;

            if (posArr[i * 3 + 1] > 40) posArr[i * 3 + 1] = -40;
            if (posArr[i * 3 + 1] < -40) posArr[i * 3 + 1] = 40;

            if (posArr[i * 3 + 2] > 40) posArr[i * 3 + 2] = -40;
            if (posArr[i * 3 + 2] < -40) posArr[i * 3 + 2] = 40;
        }

        particles.geometry.attributes.position.needsUpdate = true;

        // Gentle rotation
        scene.rotation.x += 0.0003;
        scene.rotation.y += 0.0003;

        // Mouse Parallax
        const targetRotX = mouseY * 0.05;
        const targetRotY = mouseX * 0.05;

        scene.rotation.x += (targetRotX - scene.rotation.x) * 0.02;
        scene.rotation.y += (targetRotY - scene.rotation.y) * 0.02;

        renderer.render(scene, camera);
    };

    animate();

    // 8) Resize Handler
    window.addEventListener('resize', () => {
        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    });
});
