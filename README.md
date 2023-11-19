# LoLModule-Backend

## Name
**The Tracking Fellowship** League of Legends module backend solution.

## Description
This is the part of the application that communicates with the Riot API and returns the formatted data for the requested user to the dashboard.

## Installation
This module can be installed by running the docker-compose file containing the other modules.

    lol-module:
        image: broc1603/lol-module:latest
        ports:
          - 3600:80
        environment:
          API_KEY_LOL: <API key needed to make calls to Riot API>
        networks:
          - back-tier

      mongodb:
        image: mongo:latest
        container_name: mongodb
        restart: always
        environment:
            MONGO_INITDB_ROOT_USERNAME: admin
            MONGO_INITDB_ROOT_PASSWORD: <password>
            MONGO_INITDB_DATABASE: TheTrackingFellowship
        ports:
          - 27017:27017
        volumes:
          - mongo-data:/data/db
          - ./mongo-init.js:/docker-entrypoint-initdb.d/mongo-init.js:ro
        networks:
          - back-tier


A mongoDB database needs to be running with the following setup script:


    db.createUser(
        {        
            user: "admin",        
            pwd: "<password>",
            roles: [
                {
                    role: "readWrite",
                    db: "TheTrackingFellowship"
                }
            ]
        }
    );
    db.createCollection("PlayerAccount")
    db.createCollection("TFTMatches")
    db.createCollection("LoLMatches")
    db.createCollection("LoRMatches")

### Environment Variables
* MONGODB_CONNECTION_STRING : mongodb://{_username_}:{_password_}@localhost:27017/?authSource={_username_}

## Usage
The purpose of this solution is to communicate with the Riot Games Developper API, to have access to the data on users and matches for the League of Legends video game. The module offers different endpoints to fetch different data. This module also takes care of interpreting all the raw data given by the API by calculating some useful and meaningful data for users. This module will not be accessed by the user, it will receive requests from the Dashboard and will respond with the organized data that was requested.

| Type  | Controller | Route                | Input Field               | Response Model            |     
|-------|------------|----------------------|---------------------------|---------------------------|
| GET   | LoL        | getStatsForPlayer    | summonerName (string)            | [BasicStats](#basicstats) |    
| GET   | LoL        | getChampionWinRate   | summonerName, champName (string) | championWinRate (double)        |     
| GET   | LoL        | getLaneWinRate       | summonerName, lane (string)     | laneWinRate (double)             |
| GET   | LoL        | userExists           | summonerName (string)            | userExists (string)             |

### Response

#### BasicStats



| Name                  | Type                      |
|-----------------------|---------------------------|
| summoner              | [SummonerResponse](https://developer.riotgames.com/apis#summoner-v4/GET_getByRSOPUUID:~:text=Return%20value%3A%20SummonerDTO-,SummonerDTO,-%2D%20represents%20a%20summoner)          |
| leagueEntry           | List<[LeagueEntryResponse](#leagueentryresponse)>|
| playerName            | string                    |
| profileIconId         | int                       |
| mostPlayedPosition    | string                    |
| championStats         | List<[ChampionStats](#championstats)>       |
| matchHistory          | List<[MatchResponse](https://developer.riotgames.com/apis#match-v5/GET_getMatch:~:text=Return%20value%3A%20MatchDto-,MatchDto,-NAME)>      |


#### ChampionStats

| Name          | Type      |
|---------------|-----------|
| championName  | int       |
| winRatio      | string    |
| CS            | double    |
| CSPerMinute   | double    |
| KDA           | double    |
| averageKill   | double    |
| averageDeath  | double    |
| averageAssist | double    |
| gamesPlayed   | int       |

#### LeagueEntryResponse

| Name          | Type      |
|---------------|-----------|
| leagueId      | string    |
| queueType     | string    |
| tier          | string    |
| rank          | string    |
| summonerId    | string    |
| summonerName  | string    |
| leaguePoints  | int       |
| wins          | int       |
| losses        | int       |
| veteran       | bool      |
| inactive      | bool      |
| freshBlood    | bool      |
| hotStreak     | bool      |
| winRatio      | double    |


## Roadmap
There is no specific Roadmap past the initial scope of the project plan established for our Final Degree Project.  

The project and its structure is open for additional game module, allowing further development for after Degree project or for other team final degree project.

## Authors and acknowledgment
### Authors
* Catherine Bronsard
* David Goulet-Paradis
* Simon Lacroix
* Antoine Toutant
### Acknowledgment
* Mikaël Fortin, Project Supervisor 

## License
For open source projects, say how it is licensed.

## Project status
**In development**
