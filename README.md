# Internationalbusiness

This project was generated Net CORE.

## EndPoints

Run `http://localhost:52825/api/Transaction/` to get all Transactions.
Run `http://localhost:52825/api/Transaction/{sku}` to get all Transactions by sku (i.e. I9478). This endpoints will filter all the transactions based on the given SKU and convert all currencies to Euro

Run `http://localhost:52825/api/Currency/` to get all Currencies.
Run `http://localhost:52825/api/Currency/{currencyType}` to get all Currencies based on a type given.

## BackUp Files

There are 2 files where the Data from the Currency and Transaction Endpoints data are saved:
`bkpCurrency.json`
`bkpTransaction.json`

The data is saved in this files after a success response from the endpoints and in case of somethings is not found in the API the code will try to find also in this files.

