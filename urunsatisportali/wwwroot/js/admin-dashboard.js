// SignalR connection setup
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/general-hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Start connection
connection.start().then(function () {
    console.log("SignalR Connected.");
}).catch(function (err) {
    return console.error(err.toString());
});

// Update dashboard when a new sale is received
connection.on("ReceiveSaleUpdate", function (data) {
    console.log("New sale received:", data);
    
    // Update Total Sales Count
    var totalSalesElem = document.getElementById("totalSalesCount");
    if (totalSalesElem) {
        var currentCount = parseInt(totalSalesElem.innerText.replace(/[^0-9]/g, '')) || 0;
        totalSalesElem.innerText = currentCount + 1;
        highlightElement(totalSalesElem);
    }

    // Update Total Revenue
    var totalRevenueElem = document.getElementById("totalRevenueAmount");
    if (totalRevenueElem) {
        // Parse "1.234,56 ₺" -> 1234.56
        var currentText = totalRevenueElem.innerText;
        var numericText = currentText.replace('₺', '').replace(/\./g, '').replace(',', '.').trim();
        var currentRevenue = parseFloat(numericText) || 0;
        
        var newRevenue = currentRevenue + data.totalAmount;
        
        // Format back to Turkish currency
        var formatter = new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: 'TRY'
        });
        totalRevenueElem.innerText = formatter.format(newRevenue);
        highlightElement(totalRevenueElem);
    }

    // Show toast notification
    showToast(data.message);
    
    // If there is a recent sales table, add the new row (simplified: just reload or append if detailed data sent)
    // For now, reload the page after a short delay is the simplest way to refresh lists if the user is on that page
    // setTimeout(() => location.reload(), 2000); 
});

function highlightElement(element) {
    element.style.transition = "background-color 0.5s";
    var originalColor = element.style.backgroundColor;
    element.style.backgroundColor = "#d4edda"; // Light green
    setTimeout(function() {
        element.style.backgroundColor = originalColor;
    }, 1000);
}

function showToast(message) {
    // Create toast container if not exists
    var container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(container);
    }

    var toastHtml = `
        <div class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-wallet2"></i> ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    var tempDiv = document.createElement('div');
    tempDiv.innerHTML = toastHtml.trim();
    var toastEl = tempDiv.firstChild;
    container.appendChild(toastEl);

    var toast = new bootstrap.Toast(toastEl);
    toast.show();
    
    // Auto remove from DOM after hide
    toastEl.addEventListener('hidden.bs.toast', function () {
        toastEl.remove();
    });
}
