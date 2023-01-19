async function getCoins() {
    let coins = ""
    let obj;
    const res = await fetch('https://localhost:7046/User/getSubscribedCoins') 
    obj = await res.json();
    for (let i = 0; i < obj.length; i++) {
        coins += obj[i] + ";"
    }
    getNotify(coins)
}
getCoins()
async function getNotify(coins) {
    await fetch("https://localhost:7046/User/getNotification/" + coins)
}

let coinRow = document.getElementById("coinRow");
console.log("a");
fetch("https://localhost:7046/User/getSpecificCoins")
    .then(p => {
        p.json().then(data => {
            for (let i = 0; i < data.length; i++) {
                coinRow.innerHTML += makeCoin(data[i].name, data[i].symbol, data[i].price, data[i].image);
            }
        })
    })

getNewPrices()
function getNewPrices() {


    fetch("https://localhost:7046/User/getSpecificCoins")
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
            })
        });
    setTimeout(getNewPrices, 30000)
} 

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/coinHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

start();

let counter1 = 0;
let notify = document.getElementById("notification");

function makenotify(message, count) {
    notify.innerHTML += makeNotification(count, message);
}
let m = ""
connection.on("ReceiveMessage", (message) => {


    makenotify(message, counter1)
    counter1++;
});

let coinR = `    <tr>
                    <td><img src="$image$"></td>
                    <td>$name$</td> 
                    <td>$symbol$</td> 
                    <td id="price$symbol$">$price$ USD</td>
                    <td style="max-width:200px"><canvas id="myChart$symbol$" style="position: relative; height:1vh; width:2vw"></canvas></td>
                </tr>`;

function makeCoin(name, symbol, price, image) {
    let result = coinR.replace("$image$", image)
        .replace("$name$", name)
        .replaceAll("$symbol$", symbol)
        .replace("$price$", price)

    return result;
}

let notification = `    <div id="notification:$counter1$" class="toast show" role="alert" aria-live="assertive" aria-atomic="true">
                            <div class="toast-header">
                                <strong class="me-auto">Obavestenje</strong>
                                <small>$time$</small>
                                <button type="button" class="btn-close" data-bs-dismiss="toast">
                            </div>
                            <div class="toast-body">
                                $message$
                            </div>
                        </div>`

function makeNotification(counter1, message) {
    var today = new Date();
    var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
    let result = notification.replace("$counter1$", counter1)
        .replace("$time$", time)
        .replace("$message$", message)

    return result;
}


