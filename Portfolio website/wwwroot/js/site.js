// ============================================
// PORTFOLIO WEBSITE - MAIN JAVASCRIPT
// ============================================

// Theme Toggle
(function initTheme() {
    const themeToggle = document.getElementById('themeToggle');
    const html = document.documentElement;

    // Check for saved theme or prefer dark
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

    if (savedTheme) {
        html.setAttribute('data-theme', savedTheme);
    } else if (prefersDark) {
        html.setAttribute('data-theme', 'dark');
    } else {
        html.setAttribute('data-theme', 'light');
    }

    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            const currentTheme = html.getAttribute('data-theme');
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            html.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
        });
    }
})();

// Mobile Menu
(function initMobileMenu() {
    const toggle = document.getElementById('mobileMenuToggle');
    const menu = document.getElementById('mobileMenu');

    if (toggle && menu) {
        toggle.addEventListener('click', () => {
            toggle.classList.toggle('active');
            menu.classList.toggle('active');
        });

        // Close menu when clicking a link
        menu.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', () => {
                toggle.classList.remove('active');
                menu.classList.remove('active');
            });
        });
    }
})();

// Navbar Scroll Effect
(function initNavbar() {
    const navbar = document.getElementById('navbar');
    if (!navbar) return;

    let lastScroll = 0;

    window.addEventListener('scroll', () => {
        const currentScroll = window.pageYOffset;

        if (currentScroll > 50) {
            navbar.style.boxShadow = 'var(--shadow-md)';
        } else {
            navbar.style.boxShadow = 'none';
        }

        lastScroll = currentScroll;
    });
})();

// Smooth Scroll for Anchor Links
(function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
})();

// Scroll Animations
(function initScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Observe elements
    document.querySelectorAll('.glass-card, .timeline-item, .skill-category, .project-card, .about-detail-item').forEach(el => {
        el.classList.add('scroll-animate');
        observer.observe(el);
    });
})();

// Project Filters
(function initProjectFilters() {
    const filterBtns = document.querySelectorAll('.filter-btn');
    const projectCards = document.querySelectorAll('.project-card');

    filterBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            // Update active button
            filterBtns.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            const filter = btn.getAttribute('data-filter');

            projectCards.forEach(card => {
                const category = card.getAttribute('data-category');
                if (filter === 'all' || category === filter) {
                    card.style.display = 'block';
                    card.style.animation = 'fadeIn 0.5s ease';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
})();

// Skill Progress Animation
(function initSkillAnimation() {
    const skillBars = document.querySelectorAll('.skill-progress');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const bar = entry.target;
                const width = bar.style.width;
                bar.style.width = '0';
                setTimeout(() => {
                    bar.style.width = width;
                }, 100);
                observer.unobserve(bar);
            }
        });
    }, { threshold: 0.5 });

    skillBars.forEach(bar => observer.observe(bar));
})();

// Typing Effect for Hero - CONTINUOUS LOOP
(function initTypingEffect() {
    const heroTitle = document.querySelector('.hero-title');
    if (!heroTitle) return;

    // Get all possible titles (for future add-ons)
    const titles = [
        heroTitle.textContent.trim()
    ];

    // You can add more titles here in future:
    // titles.push('New Title 1');
    // titles.push('New Title 2');

    let titleIndex = 0;
    let charIndex = 0;
    let isDeleting = false;
    let isWaiting = false;

    function typeWriter() {
        const currentTitle = titles[titleIndex];

        if (isWaiting) {
            setTimeout(() => {
                isWaiting = false;
                typeWriter();
            }, 2000); // Wait 2 seconds before next title
            return;
        }

        if (!isDeleting) {
            // Typing
            heroTitle.textContent = currentTitle.substring(0, charIndex + 1);
            charIndex++;

            if (charIndex === currentTitle.length) {
                // Finished typing
                isDeleting = true;
                isWaiting = true;
                setTimeout(typeWriter, 2000); // Wait before deleting
            } else {
                setTimeout(typeWriter, 50); // Typing speed
            }
        } else {
            // Deleting
            heroTitle.textContent = currentTitle.substring(0, charIndex - 1);
            charIndex--;

            if (charIndex === 0) {
                // Finished deleting
                isDeleting = false;
                titleIndex = (titleIndex + 1) % titles.length; // Loop to next title
                setTimeout(typeWriter, 500); // Pause before next title
            } else {
                setTimeout(typeWriter, 30); // Deleting speed
            }
        }
    }

    // Start typing after a delay
    setTimeout(typeWriter, 1000);
})();

// Form Validation Enhancement
(function initFormValidation() {
    const forms = document.querySelectorAll('.contact-form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const requiredFields = form.querySelectorAll('[required]');
            let isValid = true;

            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.style.borderColor = '#ef4444';
                } else {
                    field.style.borderColor = '';
                }
            });

            if (!isValid) {
                e.preventDefault();
            }
        });
    });
})();

// Console Message
console.log('%c Portfolio Website %c Ready ', 'background: linear-gradient(135deg, #6366f1, #8b5cf6); color: white; padding: 4px 8px; border-radius: 4px;', 'background: #1a1a2e; color: #6366f1; padding: 4px 8px; border-radius: 4px;');
