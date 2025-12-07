// Admin panel JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Mobile menu toggle
    const sidebar = document.querySelector('.sidebar');
    const toggleBtn = document.createElement('button');
    toggleBtn.className = 'btn btn-primary d-md-none position-fixed';
    toggleBtn.style.cssText = 'top: 10px; left: 10px; z-index: 1001;';
    toggleBtn.innerHTML = '<i class="bi bi-list"></i>';
    toggleBtn.onclick = function() {
        sidebar.classList.toggle('show');
    };
    document.body.appendChild(toggleBtn);

    // Auto-hide alerts
    setTimeout(function() {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(function(alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        });
    }, 5000);
});

