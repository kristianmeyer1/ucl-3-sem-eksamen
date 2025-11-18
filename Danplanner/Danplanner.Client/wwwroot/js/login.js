$(document).ready(function () {
    // Detect type and show relevant input
    $("#loginIdentifier").on("input", function () {
        var val = $(this).val() || "";
        if (val.includes("@")) {
            // User
            $("#userOtpGroup").show();
            $("#adminPasswordGroup").hide();
        } else if (val.length > 0) {
            // Admin
            $("#adminPasswordGroup").show();
            $("#userOtpGroup").hide();
        } else {
            // Empty
            $("#adminPasswordGroup").hide();
            $("#userOtpGroup").hide();
        }
    });

    // Login form submit
    $("#loginForm").submit(function (e) {
        e.preventDefault();
        var identifier = $("#loginIdentifier").val().trim();
        if (!identifier) {
            alert("Please enter Email or Admin ID.");
            return;
        }

        // Determine if user or admin
        var payload, url;
        if (identifier.includes("@")) {
            // User login via OTP
            var code = $("#loginCode").val().trim();
            if (!code) { alert("Please enter OTP code."); return; }
            payload = { Email: identifier, Code: code };
            url = "/api/auth/user/verify-code";
        } else {
            // Admin login via password
            var password = $("#loginPassword").val().trim();
            if (!password) { alert("Please enter password."); return; }
            payload = { AdminId: identifier, Password: password };
            url = "/api/auth/login";
        }

        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function (res) {
                if (res === "OTP_SENT") {
                    alert("OTP sent to your email!");
                } else {
                    alert("Logged in successfully!");
                    $("#loginModal").modal("hide"); // close modal
                    location.reload();
                }
            },
            error: function (xhr) {
                if (xhr.responseJSON && xhr.responseJSON.error) {
                    alert(xhr.responseJSON.error);
                } else {
                    alert("Invalid credentials or code.");
                }
            }
        });
    });

    // Logout
    $("#logoutBtn").click(function () {
        $.ajax({
            type: "POST",
            url: "/api/auth/logout",
            success: function () { location.reload(); },
            error: function () { alert("Logout failed."); }
        });
    });
});
