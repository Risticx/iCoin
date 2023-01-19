
let coinRow = document.getElementById("coinRow");

function subscribeCoin(clicked_button) {
    let buttonClicked = clicked_button.replace("subscribe:", "");

    fetch("https://localhost:7046/User/subscribeCoin/" + buttonClicked, {
        method: "POST"
    })
        .then(response => {
            if (response.status == 400) {
                location.href = "https://localhost:7046/Login"
            }
            if (response.status == 200) {
                getSubscribedCoins()
            }
        })

}

fetch("https://localhost:7046/api/Coin/getAllCoins")
    .then(p => {
        p.json().then(data => {
            for (let i = 0; i < data.length; i++) {
                coinRow.innerHTML += makeCoin(data[i].name, data[i].symbol, data[i].price, data[i].image);
            }
        })
    })

getNewPrices()
function getNewPrices()
{

 

    fetch("https://localhost:7046/api/Coin/getAllCoins")
        .then(p => {
            p.json().then(data => {
                for (let i = 0; i < data.length; i++) {
                    let price = document.getElementById("price" + data[i].symbol);
                    price.innerHTML = data[i].price;

                    let xV = []
                    let yV = []

                    fetch("https://localhost:7046/api/Coin/getCoinHistory" + "/" + data[i].symbol)
                        .then(p => {
                            p.json().then(coinHistory => {


                                for (let i = 0; i < coinHistory.priceAndDateTime.length; i++) {
                                    var temp = coinHistory.priceAndDateTime[i].split(";")
                                    xV.push(temp[0].replace("time:", ""));
                                    yV.push(temp[1].replace("price:", ""));

                                }
                                createChart("myChart" + data[i].symbol, xV, yV);
                            })
                        });

                    function createChart(id, xV, yV) {
                        new Chart(id, {
                            type: "line",
                            data: {
                                labels: xV,
                                datasets: [{
                                    borderColor: "#eea123",
                                    fill: false,
                                    data: yV
                                }]
                            },
                            options: {
                                response: false,
                                legend: { display: false },
                                scales: {
                                    xAxes: [{ display: false }],
                                    yAxes: [{ display: false }]
                                }
                            }
                        });
                    }
                }

                return fetch("https://localhost:7046/User/isLoggedIn/")
            })
                .then(response => {
                    if (response.status == 400) {
                    }
                    if (response.status == 200) {
                        getSubscribedCoins()
                    }
                });
        });
    setTimeout(getNewPrices, 30000)
}

function getSubscribedCoins() {
    fetch("https://localhost:7046/User/getSubscribedCoins")
        .then(response => {
            if (response.status == 200) {
                response.json().then(data => {
                    for (let i = 0; i < data.length; i++) {
                        let btn = document.getElementById("subscribe:" + data[i]);
                        btn.classList.add("disabled");
                    }
                })
            }
            
        })
}

let coinR =`    <tr>
                    <td><img src="$image$"></td>
                
                    <td>$name$</td> 
                    <td>$symbol$</td> 
                    <td id="price$symbol$">$price$ USD</td>
                    <td style="max-width: 200px"><canvas id="myChart$symbol$" style="position: relative; height:1vh; width:2vw"></canvas></td>
                    <td><button id="subscribe:$symbol$" onClick="subscribeCoin(this.id)" class="btn btn-primary1 text-uppercase fw-bold">Subscribe</button></td> 

                </tr>`;

function makeCoin(name, symbol, price, image)
{
    let result = coinR.replace("$image$", image)
        .replace("$name$", name)
        .replaceAll("$symbol$", symbol)
        .replace("$price$", price)

    return result;
}
