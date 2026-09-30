// Topbar date
document.addEventListener("DOMContentLoaded", function () {
    var el = document.getElementById("topbarDate");
    if (el) el.textContent = new Date().toLocaleDateString("en-US", { weekday: "short", month: "short", day: "numeric", year: "numeric" });

    // Sidebar toggle
    var toggle = document.getElementById("sidebarToggle");
    var sidebar = document.getElementById("sidebar");
    if (toggle && sidebar) {
        toggle.addEventListener("click", function () { sidebar.classList.toggle("open"); });
        document.addEventListener("click", function (e) {
            if (sidebar.classList.contains("open") && !sidebar.contains(e.target) && e.target !== toggle) sidebar.classList.remove("open");
        });
    }

    // Auto-dismiss alerts
    setTimeout(function () {
        document.querySelectorAll(".alert").forEach(function (a) {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(a);
            if (bsAlert) bsAlert.close();
        });
    }, 5000);
});
