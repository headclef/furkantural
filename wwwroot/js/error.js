(function syncTheme() {
    const savedTheme = localStorage.getItem("theme");
    const htmlTag = document.documentElement;
    if (savedTheme === "light" || savedTheme === "dark") {
        htmlTag.setAttribute("data-theme", savedTheme);
    } else {
        htmlTag.setAttribute("data-theme", "dark");
    }
})();

// ===== URL parametrelerini oku
function getQueryParams() {
    const params = new URLSearchParams(window.location.search);
    return {
        code: params.get("code"),
        msg: params.get("msg"),
        detail: params.get("detail")
    };
}

// ===== Durumu yaz
(function renderError() {
    const { code, msg, detail } = getQueryParams();

    const errCodeEl = document.getElementById("errCode");
    const errLabelEl = document.getElementById("errLabel");
    const errMessageEl = document.getElementById("errMessage");
    const errDescEl = document.getElementById("errDesc");
    const techHintEl = document.getElementById("techHint");

    // Varsayılanlar
    let displayCode = "500";
    let displayLabel = "Internal Server Error";
    let displayMsg = "Beklenmeyen bir hata oluştu.";
    let displayDesc = "İstediğin işlem tamamlanamadı. Bu durum kalıcı değilse birazdan tekrar dene. Eğer bu ekran sürekli çıkıyorsa geliştiricimize haber ver.";

    // Eğer parametre geldiyse override
    if (code) {
        displayCode = code;
        // Bazı klasik kodları otomatik isimlendirelim:
        switch (code) {
            case "400": displayLabel = "Bad Request"; break;
            case "401": displayLabel = "Unauthorized"; break;
            case "403": displayLabel = "Forbidden"; break;
            case "404": displayLabel = "Not Found"; break;
            case "408": displayLabel = "Request Timeout"; break;
            case "429": displayLabel = "Too Many Requests"; break;
            case "500": displayLabel = "Internal Server Error"; break;
            case "502": displayLabel = "Bad Gateway"; break;
            case "503": displayLabel = "Service Unavailable"; break;
            case "504": displayLabel = "Gateway Timeout"; break;
            default:
                displayLabel = "Error";
                break;
        }
    }

    if (msg) {
        displayMsg = decodeURIComponent(msg);
    }

    if (detail) {
        displayDesc = decodeURIComponent(detail);
    }

    errCodeEl.textContent = displayCode;
    errLabelEl.textContent = displayLabel;
    errMessageEl.textContent = displayMsg;
    errDescEl.textContent = displayDesc;

    // küçük teknik not (log amaçlı)
    techHintEl.textContent = `Status: ${displayCode} ${displayLabel}`;
})();

// ===== Geri dön butonu
document.getElementById("btnBack").addEventListener("click", () => {
    if (window.history.length > 1) {
        history.back();
    } else {
        // history yoksa fallback ana sayfa
        window.location.href = "index.html";
    }
});