## How to Build Krate Proof-of-Concept
1. Navigate into /src and open Stratis.Bitcoin.FullNode.sln with Visual Studio
2. Inside Visual Studio, in the Solution Explorer side window, right-click "Krate.KrateD" project and select "Set as StartUp Project"
3. In the top menu bar, select Build > Rebuild Solution

## How to Run Krate
1. Go into the folder that contains "Krate.KrateD.dll" file.
2. Open Windows command prompt
3. Change the directory by typing `cd <directory>`, replacing \<directory> with the directory path that contains "Krate.KrateD.dll"
4. Type `dotnet Krate.KrateD.dll`
5. Let it finish loading, and then open a browser and navigate to http://localhost:37220/swagger/index.html

### Create a wallet
1. Ensure http://localhost:37220/swagger/index.html is opened in a browser
2. Scroll to "/api/Wallet/create" and click on it. Try it out, and use something like this:
```
{
  "mnemonic": "",
  "password": "<pass>",
  "passphrase": "",
  "name": "<wallet-name>"
}
```
Replacing \<pass> and \<wallet-name> to what you wish. Leave mnemonic blank, but if you want one, ensure it is created using /api/Wallet/mnemonic above the /api/Wallet/create
Click "Execute" to create the wallet.
3. Stop running the node by going back to the command prompt and pressing `Ctrl-C`

4. Open Windows File Explorer and navigate to "%appdata%\StratisNode\krate\Main"
5. Open "krate.conf" and "\<wallet-name>.wallet.json" in a text editor
6. In \<wallet-name>.wallet.json find the "accounts" array item with the name "account 0", which will look like this:
```
"accounts": [
        {
          "index": 0,
          "name": "account 0",
          "hdPath": "m/44'/0'/0'",
          "extPubKey": "...",
          "creationTime": "1546460539",
          "externalAddresses": [
            {
              "index": 0,
              "scriptPubKey": "...",
              "pubkey": "...",
              "address": "1KieaJZtcWcUf3WdU6FtM5oaXfpkNMxt1E",
              "hdPath": "m/44'/0'/0'/0/0",
              "transactions": []
            },
			...
```
7. In the "externalAddresses", copy the first item's "address" (in the example above, "1KieaJZtcWcUf3WdU6FtM5oaXfpkNMxt1E")
8. Switch to krate.conf text file
9. Change the mining address from `#mineaddress=<string>` `mineaddress=1KieaJZtcWcUf3WdU6FtM5oaXfpkNMxt1E`
10. Enable mining by changing `#mine=0` to `mine=1`
11. Switch back to command prompt and type "dotnet run" to restart the node. This time mining will take place.

### Connecting to another node.
1. Start a new machine on the same local network.
2. Follow the steps above to create the wallet, and edit the "krate.conf" file to set the mining address.
3. Use `dotnet Krate.KrateD.dll -connect=<ip-address>`
The new node should now connect to the one already running at the \<ip-address>

### Creating a transaction

Assuming there is coin in your wallet, whether from mining or from others sending coin to your account:

1. Using the Swagger API, find `/api/Wallet/spendable-transactions`
2. Open it, click on "Try it out" on the right side, fill in your wallet name, and the account (the default account is "account 0").
3. Click Execute. The server response details should be a JSON object with a list of transactions.  Select one, copy the ID to your clipboard. Note the index shown as well:
```javascript
    {
      "id": "495b4fc36d2b77a89ba137f2df94fae0d7d5de72db0f295d5309e97989722419",
      "index": 1,
      "address": "1AGQDjFu6uhEPrYPUvW2RpHYX5xa26EgUj",
      "isChange": false,
      "amount": 1499000000,
      "creationTime": "1546898283",
      "confirmations": 134
    },
```
4. Find `/api/Wallet/build-transaction`. Open it, click on "Try it out", and fill out the JSON. Note that `feeAmount` and `feeType` cannot both be specified at the same time. For `outPoints`, use the ID and index from above.
5. If it is successfully run by clicking on "Execute", the response body will have a `hex` key. Copy its value.
6. Open `/api/Wallet/send-transaction`, click on "Try it out", and paste in the hex value from above in. "Click Execute".
7. Wait for two blocks to be mined to get this transaction to be confirmed. The first block will not have the transaction because it was already being mined when the transaction was sent, the next one will contain the transaction.

