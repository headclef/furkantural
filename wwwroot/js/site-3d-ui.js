/**
 * 3D Tilt Cards & Magnetic Buttons
 * Interactive UI enhancements for portfolio elements
 */
document.addEventListener('DOMContentLoaded', () => {

    // ========== CONFIGURATION ==========
    const CONFIG = {
        tilt: {
            maxRotation: 7,        // Max rotation in degrees
            perspective: 1000,      // CSS perspective value
            scale: 1.02,            // Scale on hover
            speed: 400,             // Transition speed (ms)
            glareOpacity: 0.02      // Glare effect opacity
        },
        magnet: {
            distance: 40,           // Activation distance in pixels
            strength: 0.4,          // Pull strength (0-1)
            scale: 1.1              // Scale on hover
        }
    };

    // ========== 3D TILT CARDS ==========
    const tiltCards = document.querySelectorAll('.hero-card, .skill-item, .price-card, .project-card, .card');

    tiltCards.forEach(card => {
        // Skip cards inside forms or with inputs
        if (card.querySelector('input, textarea, select')) return;

        // Add tilt wrapper styles
        card.style.transformStyle = 'preserve-3d';
        card.style.transition = `transform ${CONFIG.tilt.speed}ms cubic-bezier(0.03, 0.98, 0.52, 0.99)`;

        // Create glare element
        const glare = document.createElement('div');
        glare.className = 'tilt-glare';
        glare.style.cssText = `
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            pointer-events: none;
            border-radius: inherit;
            opacity: 0;
            background: linear-gradient(
                135deg,
                rgba(255, 255, 255, ${CONFIG.tilt.glareOpacity}) 0%,
                rgba(255, 255, 255, 0) 60%
            );
            transition: opacity 300ms ease;
        `;

        // Only add glare if card has relative/absolute positioning or set it
        if (getComputedStyle(card).position === 'static') {
            card.style.position = 'relative';
        }
        card.style.overflow = 'hidden';
        card.appendChild(glare);

        // Mouse enter
        card.addEventListener('mouseenter', () => {
            card.style.transition = 'none';
            glare.style.opacity = '1';
        });

        // Mouse move - calculate tilt
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const centerX = rect.left + rect.width / 2;
            const centerY = rect.top + rect.height / 2;

            const mouseX = e.clientX - centerX;
            const mouseY = e.clientY - centerY;

            // Calculate rotation (inverted for natural feel)
            const rotateY = (mouseX / (rect.width / 2)) * CONFIG.tilt.maxRotation;
            const rotateX = -(mouseY / (rect.height / 2)) * CONFIG.tilt.maxRotation;

            // Apply transform
            card.style.transform = `
                perspective(${CONFIG.tilt.perspective}px)
                rotateX(${rotateX}deg)
                rotateY(${rotateY}deg)
                scale3d(${CONFIG.tilt.scale}, ${CONFIG.tilt.scale}, ${CONFIG.tilt.scale})
            `;

            // Move glare based on mouse position
            const glareX = ((e.clientX - rect.left) / rect.width) * 100;
            const glareY = ((e.clientY - rect.top) / rect.height) * 100;
            glare.style.background = `
                radial-gradient(
                    circle at ${glareX}% ${glareY}%,
                    rgba(255, 255, 255, ${CONFIG.tilt.glareOpacity * 2}) 0%,
                    rgba(255, 255, 255, 0) 60%
                )
            `;
        });

        // Mouse leave - reset
        card.addEventListener('mouseleave', () => {
            card.style.transition = `transform ${CONFIG.tilt.speed}ms cubic-bezier(0.03, 0.98, 0.52, 0.99)`;
            card.style.transform = `
                perspective(${CONFIG.tilt.perspective}px)
                rotateX(0deg)
                rotateY(0deg)
                scale3d(1, 1, 1)
            `;
            glare.style.opacity = '0';
        });
    });

    // ========== MAGNETIC BUTTONS ==========
    const magneticButtons = document.querySelectorAll('.btn-primary, .btn-outline, .theme-toggle-btn');

    magneticButtons.forEach(button => {
        const originalTransform = getComputedStyle(button).transform;
        let bounds;

        // Store original position
        button.style.transition = 'transform 0.3s cubic-bezier(0.33, 1, 0.68, 1)';

        button.addEventListener('mouseenter', () => {
            bounds = button.getBoundingClientRect();
        });

        button.addEventListener('mousemove', (e) => {
            if (!bounds) bounds = button.getBoundingClientRect();

            const centerX = bounds.left + bounds.width / 2;
            const centerY = bounds.top + bounds.height / 2;

            const deltaX = e.clientX - centerX;
            const deltaY = e.clientY - centerY;

            // Calculate magnetic pull
            const moveX = deltaX * CONFIG.magnet.strength;
            const moveY = deltaY * CONFIG.magnet.strength;

            button.style.transform = `translate(${moveX}px, ${moveY}px) scale(${CONFIG.magnet.scale})`;
        });

        button.addEventListener('mouseleave', () => {
            button.style.transform = 'translate(0px, 0px) scale(1)';
        });
    });

    // ========== HOVER GLOW EFFECT FOR CARDS ==========
    const glowCards = document.querySelectorAll('.hero-card, .skill-item, .price-card, .project-card');

    glowCards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            card.style.setProperty('--glow-x', `${x}px`);
            card.style.setProperty('--glow-y', `${y}px`);
        });
    });

    // ========== PARALLAX DEPTH FOR CARD CONTENT ==========
    const depthCards = document.querySelectorAll('.hero-card, .price-card');

    depthCards.forEach(card => {
        const content = card.querySelectorAll('h2, .plan-name, .plan-price, .hero-tags');

        content.forEach((el, index) => {
            el.style.transition = 'transform 0.1s ease-out';
            el.style.transformStyle = 'preserve-3d';
        });

        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const centerX = rect.left + rect.width / 2;
            const centerY = rect.top + rect.height / 2;

            const mouseX = (e.clientX - centerX) / (rect.width / 2);
            const mouseY = (e.clientY - centerY) / (rect.height / 2);

            content.forEach((el, index) => {
                const depth = (index + 1) * 5;
                const moveX = mouseX * depth;
                const moveY = mouseY * depth;
                el.style.transform = `translateX(${moveX}px) translateY(${moveY}px) translateZ(${depth * 2}px)`;
            });
        });

        card.addEventListener('mouseleave', () => {
            content.forEach(el => {
                el.style.transform = 'translateX(0) translateY(0) translateZ(0)';
            });
        });
    });

    // ========== RIPPLE EFFECT ON BUTTON CLICK ==========
    const rippleButtons = document.querySelectorAll('.btn-primary, .btn-outline');

    rippleButtons.forEach(button => {
        button.style.position = 'relative';
        button.style.overflow = 'hidden';

        button.addEventListener('click', (e) => {
            const rect = button.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const ripple = document.createElement('span');
            ripple.className = 'btn-ripple';
            ripple.style.cssText = `
                position: absolute;
                width: 0;
                height: 0;
                border-radius: 50%;
                background: rgba(255, 255, 255, 0.4);
                transform: translate(-50%, -50%);
                left: ${x}px;
                top: ${y}px;
                animation: rippleEffect 0.6s ease-out forwards;
                pointer-events: none;
            `;

            button.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        });
    });

    // Add ripple animation to document
    if (!document.getElementById('ripple-styles')) {
        const style = document.createElement('style');
        style.id = 'ripple-styles';
        style.textContent = `
            @keyframes rippleEffect {
                0% {
                    width: 0;
                    height: 0;
                    opacity: 1;
                }
                100% {
                    width: 300px;
                    height: 300px;
                    opacity: 0;
                }
            }
            
            /* Glow effect for cards */
            .hero-card::before,
            .skill-item::before,
            .price-card::before,
            .project-card::before {
                content: '';
                position: absolute;
                top: var(--glow-y, 50%);
                left: var(--glow-x, 50%);
                width: 200px;
                height: 200px;
                background: radial-gradient(
                    circle,
                    rgba(56, 189, 248, 0.15) 0%,
                    transparent 70%
                );
                transform: translate(-50%, -50%);
                pointer-events: none;
                opacity: 0;
                transition: opacity 0.3s ease;
                border-radius: inherit;
            }
            
            .hero-card:hover::before,
            .skill-item:hover::before,
            .price-card:hover::before,
            .project-card:hover::before {
                opacity: 1;
            }
            
            [data-theme="light"] .hero-card::before,
            [data-theme="light"] .skill-item::before,
            [data-theme="light"] .price-card::before,
            [data-theme="light"] .project-card::before {
                background: radial-gradient(
                    circle,
                    rgba(14, 165, 233, 0.1) 0%,
                    transparent 70%
                );
            }
        `;
        document.head.appendChild(style);
    }

    // ========== 3D SCROLL REVEAL ANIMATIONS ==========
    const scrollRevealConfig = {
        threshold: 0.15,            // How much element needs to be visible
        rootMargin: '0px 0px -50px 0px',
        staggerDelay: 80,           // Delay between grid items (ms)
        duration: 800,              // Animation duration (ms)
        distance: 60,               // How far elements travel
        rotateX: 10,                // Initial X rotation (degrees)
        rotateY: 5,                 // Initial Y rotation (degrees)
        scale: 0.9                  // Initial scale
    };

    // Add 3D reveal styles
    const revealStyles = document.createElement('style');
    revealStyles.id = 'scroll-reveal-styles';
    revealStyles.textContent = `
        /* Initial hidden state for reveal elements */
        .reveal-3d {
            opacity: 0;
            transform: 
                perspective(1000px)
                translateY(${scrollRevealConfig.distance}px)
                rotateX(${scrollRevealConfig.rotateX}deg)
                rotateY(${scrollRevealConfig.rotateY}deg)
                scale(${scrollRevealConfig.scale});
            transition: 
                opacity ${scrollRevealConfig.duration}ms cubic-bezier(0.16, 1, 0.3, 1),
                transform ${scrollRevealConfig.duration}ms cubic-bezier(0.16, 1, 0.3, 1);
            will-change: opacity, transform;
        }

        /* Revealed state */
        .reveal-3d.revealed {
            opacity: 1;
            transform: 
                perspective(1000px)
                translateY(0)
                rotateX(0deg)
                rotateY(0deg)
                scale(1);
        }

        /* Stagger delay classes for grid items */
        .reveal-3d.stagger-1 { transition-delay: ${scrollRevealConfig.staggerDelay * 1}ms; }
        .reveal-3d.stagger-2 { transition-delay: ${scrollRevealConfig.staggerDelay * 2}ms; }
        .reveal-3d.stagger-3 { transition-delay: ${scrollRevealConfig.staggerDelay * 3}ms; }
        .reveal-3d.stagger-4 { transition-delay: ${scrollRevealConfig.staggerDelay * 4}ms; }
        .reveal-3d.stagger-5 { transition-delay: ${scrollRevealConfig.staggerDelay * 5}ms; }
        .reveal-3d.stagger-6 { transition-delay: ${scrollRevealConfig.staggerDelay * 6}ms; }
        .reveal-3d.stagger-7 { transition-delay: ${scrollRevealConfig.staggerDelay * 7}ms; }
        .reveal-3d.stagger-8 { transition-delay: ${scrollRevealConfig.staggerDelay * 8}ms; }

        /* Section header special animation - slide from left */
        .section-header.reveal-3d {
            transform: 
                perspective(1000px)
                translateX(-40px)
                translateY(30px)
                rotateY(5deg)
                scale(0.95);
        }

        .section-header.reveal-3d.revealed {
            transform: 
                perspective(1000px)
                translateX(0)
                translateY(0)
                rotateY(0deg)
                scale(1);
        }

        /* Hero section special treatment */
        .hero-left.reveal-3d {
            transform: 
                perspective(1000px)
                translateX(-60px)
                rotateY(8deg)
                scale(0.95);
        }

        .hero-left.reveal-3d.revealed {
            transform: 
                perspective(1000px)
                translateX(0)
                rotateY(0deg)
                scale(1);
        }

        .hero-card.reveal-3d {
            transform: 
                perspective(1000px)
                translateX(60px)
                rotateY(-8deg)
                scale(0.95);
        }

        .hero-card.reveal-3d.revealed {
            transform: 
                perspective(1000px)
                translateX(0)
                rotateY(0deg)
                scale(1);
        }
    `;
    document.head.appendChild(revealStyles);

    // Replace existing .reveal class with .reveal-3d
    const revealElements = document.querySelectorAll('.reveal');
    revealElements.forEach(el => {
        el.classList.remove('reveal');
        el.classList.add('reveal-3d');
    });

    // Add stagger classes to grid items
    const gridContainers = document.querySelectorAll('.skills-grid, .projects-grid, .pricing-wrap, .about-grid, .contact-grid, .social-grid');

    gridContainers.forEach(container => {
        const items = container.children;
        Array.from(items).forEach((item, index) => {
            if (!item.classList.contains('reveal-3d')) {
                item.classList.add('reveal-3d');
            }
            // Add stagger class (1-8, then wrap)
            const staggerIndex = (index % 8) + 1;
            item.classList.add(`stagger-${staggerIndex}`);
        });
    });

    // Intersection Observer for reveal
    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('revealed');
                // Stop observing once revealed (one-time animation)
                revealObserver.unobserve(entry.target);
            }
        });
    }, {
        threshold: scrollRevealConfig.threshold,
        rootMargin: scrollRevealConfig.rootMargin
    });

    // Observe all reveal elements
    document.querySelectorAll('.reveal-3d').forEach(el => {
        revealObserver.observe(el);
    });

    // Also add reveal to cards that might not have .reveal class
    const additionalRevealElements = document.querySelectorAll(
        '.skill-item:not(.reveal-3d), .project-card:not(.reveal-3d), .price-card:not(.reveal-3d), .social-card:not(.reveal-3d)'
    );

    additionalRevealElements.forEach((el, index) => {
        el.classList.add('reveal-3d');
        const staggerIndex = (index % 8) + 1;
        el.classList.add(`stagger-${staggerIndex}`);
        revealObserver.observe(el);
    });

    console.log('✨ 3D Tilt Cards, Magnetic Buttons & Scroll Reveals initialized');
});
