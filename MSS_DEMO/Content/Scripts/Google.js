function onSignIn(googleUser) {
    // Useful data for your client-side scripts:
    var profile = googleUser.getBasicProfile();
    //document.getElementById('profileinfo').innerHTML = profile.getEmail();
    var Mail = profile.getEmail();
    var formData = new FormData();
    formData.append("Mail", Mail);
    $.ajax({
        url: '/Login/LoginWithGoogle',
        type: 'POST',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message == "true") {
                window.location.href = "/Home/Index"
            }
            if (data.message == "false") {
                signOut();
                sweetAlert("Error!","Login fail!", "error");
            }

        }
    }).done(function () {
    });
}

    function onLoad() {
     gapi.load('auth2', function () {
      gapi.auth2.init();
        });
}

function signOut() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        document.getElementById('profileinfo').innerHTML = "";
       $.ajax({
            url: 'Logout/Login',
            type: 'POST',
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
            }
        }).done(function () {
        });
    });
}

 function revokeAllScopes () {
    auth2.disconnect();
}

    
        


        