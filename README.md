# Auction lot info scrapper

## Get auction lots endpoint

**Base URL**: https://neal.fun
**Method**: POST
**Auction lots path**: api/auction-game/getItems

| Field  | Description                 |
|--------|-----------------------------|
| id     | Auction lot ID              |
| title  | Auction lot name            |
| medium | Auction lot description     |
| artist | Lot author                  |
| date   | Lot creation (release) date |
| price  | Lot price                   |
| img    | Lot image ID                |
| index  | Lot index number. Not used  |

## Get auction lot image

**Base URL**: https://auction-game.neal.fun
**Method**: GET
**Auction lots path**: {Lot image ID}.jpg