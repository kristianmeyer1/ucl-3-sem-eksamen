$(function () {
    console.log("LOGIN.JS LOADED");

    const loginModalEl = document.getElementById('loginModal');
    const loginModal = new bootstrap.Modal(loginModalEl);

    // -------------------------------
    // OPEN MODAL ON LOGIN BUTTON CLICK
    // -------------------------------
    $("#loginBtn").on("click", function (e) {
        e.preventDefault();

        const identifier = $("#loginIdentifier").val().trim();
        console.log("LOGIN BTN CLICKED, identifier =", identifier);

        if (!identifier) {
            alert("Enter Email or Admin ID.");
            return;
        }

        if (identifier.includes("@")) {
            // USER → REQUEST OTP
            console.log("USER FLOW: request-code");
            $.ajax({
                type: "POST",
                url: "/api/auth/user/request-code",
                contentType: "application/json",
                data: JSON.stringify({ UserEmail: identifier }),
                success: function () {
                    console.log("REQUEST-CODE SUCCESS");
                    $("#userOtpGroup").show();
                    $("#adminPasswordGroup").hide();
                    $("#loginCode").val("").focus();
                    loginModal.show();
                    alert("OTP er sendt til din email.");
                },
                error: function (xhr) {
                    console.error("REQUEST-CODE ERROR", xhr);
                    if (xhr.status === 404) {
                        alert("Denne email er ikke registreret. Brug 'Registrer'-knappen først.");
                    } else {
                        alert("Kunne ikke sende OTP. Prøv igen senere.");
                    }
                }
            });
        } else {
            // ADMIN → SHOW PASSWORD FIELD
            console.log("ADMIN FLOW: show password");
            $("#adminPasswordGroup").show();
            $("#userOtpGroup").hide();
            $("#loginPassword").val("").focus();
            loginModal.show();
        }
    });

    // -------------------------------
    // LOGIN BUTTON I MODAL (EVENT DELEGATION)
    // -------------------------------
    $(document).on("click", "#loginSubmitBtn", function (e) {
        e.preventDefault();
        console.log("LOGIN SUBMIT BTN CLICKED");

        const identifier = $("#loginIdentifier").val().trim();
        const password = $("#loginPassword").val().trim();
        const otp = $("#loginCode").val().trim();

        console.log("identifier =", identifier, "password =", password, "otp =", otp);

        if (!identifier) {
            alert("Enter Email or Admin ID.");
            return;
        }

        // USER LOGIN → VERIFY OTP
        if (identifier.includes("@")) {
            if (!otp) {
                alert("Enter the OTP code sent to your email.");
                return;
            }

            console.log("USER FLOW: verify-code");
            $.ajax({
                type: "POST",
                url: "/api/auth/user/verify-code",
                contentType: "application/json",
                data: JSON.stringify({
                    UserEmail: identifier,
                    Code: otp
                }),
                success: function () {
                    console.log("VERIFY-CODE SUCCESS");
                    loginModal.hide();
                    location.reload();
                },
                error: function (xhr) {
                    console.error("VERIFY-CODE ERROR", xhr);
                    alert(xhr.responseText || "Incorrect OTP.");
                }
            });
        }
        // ADMIN LOGIN → ID & PASSWORD
        else {
            const adminId = parseInt(identifier, 10);
            if (isNaN(adminId) || !password) {
                alert("Enter Admin ID and password.");
                return;
            }

            console.log("ADMIN FLOW: login, adminId =", adminId);

            $.ajax({
                type: "POST",
                url: "/api/auth/login",
                contentType: "application/json",
                data: JSON.stringify({
                    AdminId: adminId,
                    Password: password
                }),
                success: function (resp) {
                    console.log("ADMIN LOGIN SUCCESS", resp);
                    loginModal.hide();
                    location.reload();
                },
                error: function (xhr) {
                    console.error("ADMIN LOGIN ERROR", xhr);
                    alert(xhr.responseText || "Admin login failed.");
                }
            });
        }
    });

    // -------------------------------
    // LOGOUT
    // -------------------------------
    $(document).on("click", "#logoutBtn", function () {
        $.ajax({
            type: "POST",
            url: "/api/auth/logout",
            success: function () {
                location.href = "/";
            },
            error: function () {
                alert("Logout failed.");
            }
        });
    });
});
