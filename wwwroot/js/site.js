// Yıl
document.getElementById("year").textContent = new Date().getFullYear();

// Tema toggle
const htmlTag = document.documentElement;
const themeBtn = document.getElementById("themeToggle");
const themeIcon = document.getElementById("themeIcon");
const themeText = themeBtn.querySelector(".toggle-text");

function applyTheme(theme) {
    htmlTag.setAttribute("data-theme", theme);
    if (theme === "dark") {
        themeIcon.textContent = "🌙";
        themeText.textContent = "Dark";
    } else {
        themeIcon.textContent = "☀️";
        themeText.textContent = "Light";
    }
}

const savedTheme = localStorage.getItem("theme");
if (savedTheme === "light" || savedTheme === "dark") {
    applyTheme(savedTheme);
} else {
    applyTheme("dark");
}

themeBtn.addEventListener("click", () => {
    const current = htmlTag.getAttribute("data-theme");
    const next = current === "dark" ? "light" : "dark";
    applyTheme(next);
    localStorage.setItem("theme", next);
});

// TOAST
const toastBox = document.getElementById("toastBox");
const toastTitle = document.getElementById("toastTitle");
const toastMsg = document.getElementById("toastMsg");

function showToast(type, title, msg, timeoutMs = 4000) {
    toastBox.classList.remove("error");
    toastBox.style.borderColor = "var(--toast-border-success)";

    if (type === "error") {
        toastBox.classList.add("error");
        toastBox.style.borderColor = "var(--toast-border-error)";
    }

    toastTitle.textContent = title;
    toastMsg.textContent = msg;

    toastBox.style.display = "block";
    toastBox.style.opacity = "1";

    // Auto hide
    setTimeout(() => {
        toastBox.style.opacity = "0";
        setTimeout(() => {
            toastBox.style.display = "none";
        }, 300);
    }, timeoutMs);
}

// CONTACT FORM
const contactForm = document.getElementById("contactForm");
const contactSubmitBtn = document.getElementById("contactSubmitBtn");

if (contactForm) {
    contactForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        const nameSurnameEl = document.getElementById("NameSurname");
        const emailEl = document.getElementById("Email");
        const messageNeedEl = document.getElementById("MessageNeed");
        const turnstileEl = document.getElementById("TurnstileResponse");

        const nameSurname = nameSurnameEl.value.trim();
        const email = emailEl.value.trim();
        const messageNeed = messageNeedEl.value.trim();
        const turnstileResponse = turnstileEl.value.trim();

        if (!nameSurname || !email || !messageNeed) {
            showToast("error", "Eksik bilgi", "Lütfen tüm alanları doldur.", 4000);
            return;
        }

        if (!turnstileResponse) {
            showToast(
                "error",
                "Doğrulama gerekli",
                "Bot olmadığını kanıtlamadan mesaj göndermene izin yok 😇",
                5000
            );
            return;
        }

        // Butonu kilitle
        const originalBtnText = contactSubmitBtn.textContent;
        contactSubmitBtn.disabled = true;
        contactSubmitBtn.textContent = "Gönderiliyor...";

        try {
            // Anti-forgery token'ı formdan çek
            const antiForgeryInput = contactForm.querySelector('input[name="__RequestVerificationToken"]');
            const antiForgeryToken = antiForgeryInput ? antiForgeryInput.value : null;

            // Model binder için klasik form-encoded body
            const formBody = new URLSearchParams({
                NameSurname: nameSurname,
                Email: email,
                MessageNeed: messageNeed,
                TurnstileResponse: turnstileResponse
            });

            const response = await fetch(contactForm.getAttribute("action"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
                    ...(antiForgeryToken
                        ? { "RequestVerificationToken": antiForgeryToken }
                        : {})
                },
                body: formBody.toString()
            });

            const serverText = await response.text();

            if (response.ok) {
                showToast(
                    "success",
                    "Mesaj alındı",
                    serverText || "Tamamdır, iletin bana ulaştı, en kısa sürede sana döneceğim!",
                    5000
                );

                // formu temizle
                contactForm.reset();

                // Turnstile token'ını da sıfırla
                turnstileEl.value = "";

                // Turnstile widget'ını resetle ki tekrar çözsün
                if (window.turnstile && typeof window.turnstile.reset === "function") {
                    const widgetContainer = document.querySelector(".cf-turnstile");
                    if (widgetContainer) {
                        window.turnstile.reset(widgetContainer);
                    }
                }
            } else {
                showToast(
                    "error",
                    "Gönderilemedi",
                    serverText || "Bir sorun oluştu. Lütfen tekrar dene.",
                    6000
                );

                // Güvenlik hatası vb. durumda da reset
                if (window.turnstile && typeof window.turnstile.reset === "function") {
                    const widgetContainer = document.querySelector(".cf-turnstile");
                    if (widgetContainer) {
                        window.turnstile.reset(widgetContainer);
                    }
                }
                turnstileEl.value = "";
            }
        } catch (err) {
            console.error("Mail gönderim hatası:", err);
            showToast(
                "error",
                "Bağlantı Hatası",
                "Sunucuya ulaşılamadı. Birazdan tekrar dene.",
                6000
            );
        } finally {
            contactSubmitBtn.disabled = false;
            contactSubmitBtn.textContent = originalBtnText;
        }
    });
}

// CONSENT MODAL LOGIC
const consentOverlay = document.getElementById("consentOverlay");
const consentOk = document.getElementById("consentOk");
const consentClose = document.getElementById("consentClose");

// helper: now + days
function futureTimestamp(days) {
    const now = new Date();
    now.setDate(now.getDate() + days);
    return now.getTime();
}

function openConsent() {
    consentOverlay.style.display = "flex";
}

function closeConsent() {
    consentOverlay.style.display = "none";
}

function saveConsentForDays(days) {
    const until = futureTimestamp(days);
    localStorage.setItem("consentUntil", until.toString());
}

// Overlay tıklayınca da kabul gibi davranıyoruz
consentOverlay.addEventListener("click", (e) => {
    // kullanıcı modal'ın dışına tıkladıysa
    if (e.target === consentOverlay) {
        saveConsentForDays(7);
        closeConsent();
    }
});

consentOk.addEventListener("click", () => {
    saveConsentForDays(7);
    closeConsent();
});

consentClose.addEventListener("click", () => {
    saveConsentForDays(7);
    closeConsent();
});

// Check consent on load
(function checkConsent() {
    const stored = localStorage.getItem("consentUntil");
    const nowTs = Date.now();

    if (!stored) {
        // hiç yok -> göster
        openConsent();
        return;
    }

    const untilTs = parseInt(stored, 10);
    if (isNaN(untilTs) || nowTs > untilTs) {
        // bozuk ya da süresi bitmiş -> tekrar sor
        openConsent();
    } else {
        // henüz süresi dolmadı -> gösterme
        closeConsent();
    }
})();

// SCROLL REVEAL LOGIC
(function () {
    const reveals = document.querySelectorAll(".reveal");

    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("active");
                // Stop observing once revealed if you want it to happen only once
                revealObserver.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.15
    });

    reveals.forEach((element) => {
        revealObserver.observe(element);
    });
})();

// ACTIVE NAVIGATION LOGIC
(function () {
    const sections = document.querySelectorAll("section");
    const navLinks = document.querySelectorAll(".nav-links a");

    const navObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                const id = entry.target.getAttribute("id");
                if (!id) return;

                navLinks.forEach((link) => {
                    link.classList.remove("active");
                    const href = link.getAttribute("href");
                    if (href === `#${id}`) {
                        link.classList.add("active");
                    }
                });
            }
        });
    }, {
        threshold: 0.55 // Highlight when 55% visible
    });

    sections.forEach((section) => {
        navObserver.observe(section);
    });
})();