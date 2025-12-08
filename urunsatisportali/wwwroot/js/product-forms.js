// Product form handling with AJAX
$(document).ready(function () {
    // Handle Create Product form
    $('#createProductForm').on('submit', function (e) {
        e.preventDefault();
        
        var form = $(this);
        var submitBtn = form.find('button[type="submit"]');
        var originalText = submitBtn.html();
        
        // Clear previous messages
        $('.alert').remove();
        $('.text-danger.field-error').remove();
        $('.is-invalid').removeClass('is-invalid');
        
        // Validate form first
        if (!form.valid()) {
            console.log('Form validation failed');
            // Show validation errors
            var firstError = form.find('.input-validation-error').first();
            if (firstError.length) {
                firstError.focus();
                showAlert('danger', 'Lütfen tüm zorunlu alanları doldurun ve geçerli değerler girin.');
            }
            return false;
        }
        
        // Check CategoryId - it should not be empty or 0
        var categorySelect = form.find('[name="CategoryId"]');
        var categoryId = categorySelect.val();
        console.log('CategoryId value:', categoryId, 'Type:', typeof categoryId);
        
        // Check if category is selected (not empty, not 0, and is a valid number > 0)
        if (!categoryId || categoryId === '' || categoryId === '0' || isNaN(categoryId) || parseInt(categoryId) <= 0) {
            showAlert('danger', 'Lütfen bir kategori seçin.');
            categorySelect.addClass('is-invalid').focus();
            return false;
        }
        
        // Disable submit button and show loading
        submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Kaydediliyor...');
        
        // Get form data
        var formData = form.serialize();
        console.log('Submitting form data:', formData);
        
        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: formData,
            dataType: 'json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                console.log('Response received:', response);
                if (response.success) {
                    // Show success message
                    showAlert('success', response.message);
                    
                    // Reset form
                    form[0].reset();
                    
                    // Redirect after 1.5 seconds
                    setTimeout(function () {
                        window.location.href = '/Admin/Products';
                    }, 1500);
                } else {
                    // Show error message
                    showAlert('danger', response.message);
                    
                    // Display field errors if any
                    if (response.errors && response.errors.length > 0) {
                        displayFieldErrors(response.errors);
                    }
                    
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = 'Bir hata oluştu. Lütfen tekrar deneyin.';
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                    // Display field errors if any
                    if (xhr.responseJSON.errors && xhr.responseJSON.errors.length > 0) {
                        displayFieldErrors(xhr.responseJSON.errors);
                    }
                } else if (xhr.responseText) {
                    // Try to parse as JSON even if content-type is wrong
                    try {
                        var jsonResponse = JSON.parse(xhr.responseText);
                        if (jsonResponse.message) {
                            errorMessage = jsonResponse.message;
                            if (jsonResponse.errors && jsonResponse.errors.length > 0) {
                                displayFieldErrors(jsonResponse.errors);
                            }
                        }
                    } catch (e) {
                        console.error('Error parsing response:', e);
                    }
                }
                showAlert('danger', errorMessage);
                submitBtn.prop('disabled', false).html(originalText);
            }
        });
    });
    
    // Handle Edit Product form
    $('#editProductForm').on('submit', function (e) {
        e.preventDefault();
        
        var form = $(this);
        var submitBtn = form.find('button[type="submit"]');
        var originalText = submitBtn.html();
        
        // Clear previous messages
        $('.alert').remove();
        $('.text-danger.field-error').remove();
        $('.is-invalid').removeClass('is-invalid');
        
        // Validate form first
        if (!form.valid()) {
            console.log('Form validation failed');
            // Show validation errors
            var firstError = form.find('.input-validation-error').first();
            if (firstError.length) {
                firstError.focus();
                showAlert('danger', 'Lütfen tüm zorunlu alanları doldurun ve geçerli değerler girin.');
            }
            return false;
        }
        
        // Check CategoryId - it should not be empty or 0
        var categorySelect = form.find('[name="CategoryId"]');
        var categoryId = categorySelect.val();
        console.log('CategoryId value:', categoryId, 'Type:', typeof categoryId);
        
        // Check if category is selected (not empty, not 0, and is a valid number > 0)
        if (!categoryId || categoryId === '' || categoryId === '0' || isNaN(categoryId) || parseInt(categoryId) <= 0) {
            showAlert('danger', 'Lütfen bir kategori seçin.');
            categorySelect.addClass('is-invalid').focus();
            return false;
        }
        
        // Disable submit button and show loading
        submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Güncelleniyor...');
        
        // Get form data
        var formData = form.serialize();
        console.log('Submitting form data:', formData);
        
        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: formData,
            dataType: 'json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                console.log('Response received:', response);
                if (response.success) {
                    // Show success message
                    showAlert('success', response.message);
                    
                    // Redirect after 1.5 seconds
                    setTimeout(function () {
                        window.location.href = '/Admin/Products';
                    }, 1500);
                } else {
                    // Show error message
                    showAlert('danger', response.message);
                    
                    // Display field errors if any
                    if (response.errors && response.errors.length > 0) {
                        displayFieldErrors(response.errors);
                    }
                    
                    // Re-enable submit button
                    submitBtn.prop('disabled', false).html(originalText);
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = 'Bir hata oluştu. Lütfen tekrar deneyin.';
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                    // Display field errors if any
                    if (xhr.responseJSON.errors && xhr.responseJSON.errors.length > 0) {
                        displayFieldErrors(xhr.responseJSON.errors);
                    }
                } else if (xhr.responseText) {
                    // Try to parse as JSON even if content-type is wrong
                    try {
                        var jsonResponse = JSON.parse(xhr.responseText);
                        if (jsonResponse.message) {
                            errorMessage = jsonResponse.message;
                            if (jsonResponse.errors && jsonResponse.errors.length > 0) {
                                displayFieldErrors(jsonResponse.errors);
                            }
                        }
                    } catch (e) {
                        console.error('Error parsing response:', e);
                    }
                }
                showAlert('danger', errorMessage);
                submitBtn.prop('disabled', false).html(originalText);
            }
        });
    });
    
    // Function to show alert messages
    function showAlert(type, message) {
        var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
            '</div>';
        
        // Insert alert at the top of the form
        $('.card-body').prepend(alertHtml);
        
        // Auto-hide after 5 seconds
        setTimeout(function () {
            $('.alert').fadeOut(function () {
                $(this).remove();
            });
        }, 5000);
    }
    
    // Function to display field-specific errors
    function displayFieldErrors(errors) {
        console.log('Displaying field errors:', errors);
        
        // Clear previous field errors
        $('.text-danger.field-error').remove();
        $('.is-invalid').removeClass('is-invalid');
        
        // Display errors for each field
        errors.forEach(function (error) {
            var fieldName = error.field;
            var errorMessage = error.message;
            
            console.log('Processing error for field:', fieldName, 'Message:', errorMessage);
            
            // Find the field - try different selectors
            var field = $('[name="' + fieldName + '"]');
            if (field.length === 0) {
                // Try with Product prefix (ASP.NET Core model binding)
                field = $('[name="Product.' + fieldName + '"]');
            }
            if (field.length === 0) {
                // Try with asp-for generated name
                field = $('#' + fieldName);
            }
            
            if (field.length) {
                field.addClass('is-invalid');
                
                // Find or create validation span
                var validationSpan = field.siblings('span[data-valmsg-for="' + fieldName + '"]');
                if (validationSpan.length === 0) {
                    validationSpan = field.siblings('span[data-valmsg-for="Product.' + fieldName + '"]');
                }
                if (validationSpan.length === 0) {
                    validationSpan = field.siblings('span.text-danger');
                }
                
                if (validationSpan.length) {
                    validationSpan.addClass('field-error').html(errorMessage).show();
                } else {
                    // Create error span if it doesn't exist
                    var errorSpan = $('<span class="text-danger field-error d-block">' + errorMessage + '</span>');
                    field.closest('.mb-3, .form-group').append(errorSpan);
                }
                
                // Scroll to first error
                if (errors.indexOf(error) === 0) {
                    $('html, body').animate({
                        scrollTop: field.offset().top - 100
                    }, 500);
                }
            } else {
                console.warn('Field not found for error:', fieldName);
            }
        });
    }
});
