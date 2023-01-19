let btn = document.getElementById("login");
btn.onclick = (ev) => this.login();

function login() {
    let user = document.getElementById("user").value;
    let pass = document.getElementById("pass").value;

    let errorBox = document.getElementById("error");
    let successBox = document.getElementById("success");

    fetch("https://localhost:7046/Home/Login/" + user + "/" + pass, {
        method: "POST"
    }).then(response => {

        if (response.status == 400) {
            errorBox.classList.remove("d-none");
            errorBox.innerHTML = "Pogresan user ili sifra!";
        }

        if (response.status == 404) {
            errorBox.classList.remove("d-none");
            errorBox.innerHTML = "Morate popuniti sva polja!";
        }

        if (response.status == 200) {
            successBox.classList.remove("d-none");
            errorBox.classList.add("d-none");
            successBox.innerHTML = "Uspesan login!";
            setTimeout(function () {
                window.location = 'https://localhost:7046/Index';
            }, 2000)
        }
    });
}