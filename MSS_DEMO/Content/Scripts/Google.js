function onSignIn(googleUser) {
    // Useful data for your client-side scripts:
    var profile = googleUser.getBasicProfile();
    //document.getElementById('profileinfo').innerHTML = profile.getEmail();
    var Mail = profile.getEmail();
    var formData = new FormData();
    formData.append("Mail", Mail);
    $.ajax({
        url: 'LoginWithGoogle/Login',
        type: 'POST',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (data) {  
        }
    }).done(function () {
    });
}
function signOut() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        document.getElementById('profileinfo').innerHTML = "";
    })
    window.location.href = '@Url.Action("Logout", "Login")'
   
}