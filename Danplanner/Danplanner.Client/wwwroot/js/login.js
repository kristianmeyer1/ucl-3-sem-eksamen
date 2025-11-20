$(function () {
    const loginModalEl = document.getElementById('loginModal');
    const loginModal = new bootstrap.Modal(loginModalEl);

    $("#loginBtn").click(function (e) {
        e.preventDefault(); // prevent form default if inside form

        const identifier = $("#loginIdentifier").val().trim();
        if (!identifier) {
            alert("Enter Email or Admin ID.");
            return;
        }

        if (identifier.includes("@")) {
            // User login → request OTP
            $.ajax({
                type: "POST",
                url: "/api/auth/user/request-code",
                contentType: "application/json",
                data: JSON.stringify({ UserEmail: identifier }),
                success: function () {
                    $("#userOtpGroup").show();
                    $("#adminPasswordGroup").hide();
                    $("#loginCode").val("").focus();
                    loginModal.show(); // manually open modal
                    alert("OTP sent to your email!");
                },
                error: function (xhr) {
                    console.error(xhr);
                    alert("Failed to send OTP.");
                }
            });
        } else {
            // Admin login → show password input
            $("#adminPasswordGroup").show();
            $("#userOtpGroup").hide();
            loginModal.show(); // manually open modal
        }
    });
});
