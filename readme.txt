Run the application as docker image:
1. open powershell
2. cd to project root
3. docker compose up
4. application running on http://localhost:8080

Debug the application with visual studio:
1. set Single Startup project "Casino"
2. launch http profile
3. application running at http://localhost:5269


API:
- no authentication
- use guid {userid} as query parameter of the wallet

Register user
POST http://localhost:8080/api/wallet/{userid}/register

Get current balance of the user
GET http://localhost:8080/api/wallet/{userid}/balance

Get transactions of the user
GET http://localhost:8080/api/wallet/{userid}/transactions

Create new transaction for the user
POST http://localhost:8080/api/wallet/{userid}/transactions

{
    "id": "58f551ca-eb56-4131-a5cd-c1d29b21d9ae",
    "type": 2,
    "value": 5
}

Body parameters
"id": unique identifier of the transaction
"type": type of the transaction (Deposit=1, Stake=2, Win=3)
"value": amount