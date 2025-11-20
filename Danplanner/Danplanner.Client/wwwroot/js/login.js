$(document).ready(function () {
    // Detect type and show relevant input
    $("#loginIdentifier").on("input", function () {
        var val = $(this).val() || "";
        if (val.includes("@")) {
            // User email
            $("#userOtpGroup").hide(); // hide initially
            $("#adminPasswordGroup").hide();
        } else if (val.length > 0) {
            // Admin ID
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

        if (identifier.includes("@")) {
            // User login via OTP
            var code = $("#loginCode").val().trim();

            if (!code) {
                // Step 1: Request OTP
                $.ajax({
                    type: "POST",
                    url: "/api/auth/user/request-code",
                    contentType: "application/json",
                    data: JSON.stringify({ UserEmail: identifier }),
                    success: function () {
                        alert("OTP sent to your email!");
                        $("#userOtpGroup").show();
                        $("#loginCode").focus();
                    },
                    error: function () {
                        alert("Failed to send OTP. Please try again.");
                    }
                });
            } else {
                // Step 2: Verify OTP
                $.ajax({
                    type: "POST",
                    url: "/api/auth/user/verify-code",
                    contentType: "application/json",
                    data: JSON.stringify({ UserEmail: identifier, Code: code }),
                    success: function () {
                        alert("Logged in successfully!");
                        $("#loginModal").modal("hide");
                        location.reload();
                    },
                    error: function () {
                        alert("Invalid OTP code. Please try again.");
                    }
                });
            }
        } else {
            // Admin login
            var password = $("#loginPassword").val().trim();
            if (!password) {
                alert("Please enter password.");
                return;
            }

            $.ajax({
                type: "POST",
                url: "/api/auth/login",
                contentType: "application/json",
                data: JSON.stringify({ AdminId: parseInt(identifier), Password: password }),
                success: function () {
                    alert("Logged in successfully!");
                    $("#loginModal").modal("hide");
                    location.reload();
                },
                error: function () {
                    alert("Invalid credentials. Please try again.");
                }
            });
        }
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
