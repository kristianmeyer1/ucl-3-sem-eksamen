$(function () {
    // Prefill email in registration modal from navbar
    $("#registerBtn").click(function () {
        const emailFromNavbar = $("#loginIdentifier").val().trim();
        if (emailFromNavbar && emailFromNavbar.includes("@")) {
            $("#registerEmail").val(emailFromNavbar);
        }
    });

    // Register form submission
    $("#registerForm").submit(function (e) {
        e.preventDefault();

        var username = $("#registerUsername").val().trim();
        var email = $("#registerEmail").val().trim();
        var mobile = $("#registerMobile").val().trim();
        var address = $("#registerAddress").val().trim();

        if (!username || !email || !mobile || !address) {
            alert("Please fill in all fields.");
            return;
        }

        $.ajax({
            type: "POST",
            url: "/api/auth/register",
            contentType: "application/json",
            data: JSON.stringify({
                Username: username,
                Email: email,
                MobilePhone: mobile,
                Address: address
            }),
            success: function () {
                alert("Registration successful!");
                $("#registerModal").modal("hide");
            },
            error: function (xhr) {
                console.error(xhr);
                alert("Registration failed. Please try again.");
            }
        });
    });
});