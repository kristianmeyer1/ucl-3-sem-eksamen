$(function () {
    const loginModalEl = document.getElementById('loginModal');
    const loginModal = new bootstrap.Modal(loginModalEl);

    // -------------------------------
    // OPEN LOGIN MODAL
    // -------------------------------
    $("#loginBtn").click(function (e) {
        e.preventDefault();

        const identifier = $("#loginIdentifier").val().trim();
        if (!identifier) {
            alert("Enter Email or Admin ID.");
            return;
        }

        if (identifier.includes("@")) {
            // USER → Request OTP
            $.ajax({
                type: "POST",
                url: "/api/auth/user/request-code",
                contentType: "application/json",
                xhrFields: { withCredentials: true },
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
            // ADMIN → Show password
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
            // USER LOGIN → Verify OTP
            if (!otp) {
                alert("Enter the OTP code sent to your email.");
                return;
            }

            $.ajax({
    type: "POST",
    url: "/api/auth/user/verify-code",
    contentType: "application/json",
    xhrFields: { withCredentials: true },
    data: JSON.stringify({ UserEmail: identifier, Code: otp }),
    success: function (token) {
        loginModal.hide();
        location.reload();
    },
    error: function () {
        alert("Incorrect OTP or OTP expired.");
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
                xhrFields: { withCredentials: true },
                data: JSON.stringify({ AdminId: adminId, Password: password }),
                success: function (res) {
                    if (res.status === "OK") {
                        loginModal.hide();
                        location.reload();
                    } else {
                        alert("Admin login failed.");
                    }
                },
                error: function () {
                    alert("Admin login failed. Check your ID and password.");
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
            xhrFields: { withCredentials: true },
            success: function () {
                location.href = "/";
            },
            error: function () {
                alert("Logout failed.");
            }
        });
    });
});
