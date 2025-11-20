$(function () {
    const loginModalEl = document.getElementById('loginModal');
    const loginModal = new bootstrap.Modal(loginModalEl);

    // -------------------------------
    // OPEN MODAL ON LOGIN BUTTON CLICK
    // -------------------------------
    $("#loginBtn").click(function (e) {
        e.preventDefault();

        const identifier = $("#loginIdentifier").val().trim();
        if (!identifier) {
            alert("Enter Email or Admin ID.");
            return;
        }

        if (identifier.includes("@")) {
            // USER → REQUEST OTP
            $.ajax({
                type: "POST",
                url: "/api/auth/user/request-code",
                contentType: "application/json",
                data: JSON.stringify({ UserEmail: identifier }),
                success: function () {
                    $("#userOtpGroup").show();
                    $("#adminPasswordGroup").hide();
                    $("#loginCode").val("").focus();
                    loginModal.show();
                    alert("OTP has been sent to your email.");
                },
                error: function () {
                    alert("Failed to send OTP.");
                }
            });
        } else {
            // ADMIN → SHOW PASSWORD FIELD
            $("#adminPasswordGroup").show();
            $("#userOtpGroup").hide();
            $("#loginPassword").val("").focus();
            loginModal.show();
        }
    });

    // -------------------------------
    // LOGIN FORM SUBMIT
    // -------------------------------
    $("#loginForm").submit(function (e) {
        e.preventDefault();

        const identifier = $("#loginIdentifier").val().trim();
        const password = $("#loginPassword").val().trim();
        const otp = $("#loginCode").val().trim();

        if (identifier.includes("@")) {
            // USER LOGIN → VERIFY OTP
            if (!otp) {
                alert("Enter the OTP code sent to your email.");
                return;
            }

            $.ajax({
                type: "POST",
                url: "/api/auth/user/verify-code",
                contentType: "application/json",
                data: JSON.stringify({
                    UserEmail: identifier,
                    Code: otp
                }),
                success: function () {
                    loginModal.hide();
                    location.reload(); // logged in → refresh UI
                },
                error: function () {
                    alert("Incorrect OTP.");
                }
            });
        } else {
            // ADMIN LOGIN → ID & PASSWORD
            const adminId = parseInt(identifier, 10);
            if (isNaN(adminId) || !password) {
                alert("Enter Admin ID and password.");
                return;
            }

            $.ajax({
                type: "POST",
                url: "/api/auth/login",
                contentType: "application/json",
                data: JSON.stringify({
                    AdminId: adminId,
                    Password: password
                }),
                success: function () {
                    loginModal.hide();
                    location.reload(); // logged in → refresh UI
                },
                error: function (xhr) {
                    console.error(xhr);
                    alert("Admin login failed.");
                }
            });
        }
    });

    // -------------------------------
    // LOGOUT
    // -------------------------------
    $("#logoutBtn").click(function () {
        $.ajax({
            type: "POST",
            url: "/api/auth/logout",
            success: function () {
                location.href = "/"; // go to homepage after logout
            },
            error: function () {
                alert("Logout failed.");
            }
        });
    });
});
