CREATE TABLE IF NOT EXISTS gamestore.users
(
    nickname           varchar,
    steam_profile_link varchar,
    wishlist           list<varchar>,
    owned_games        list<varchar>,
    friends            list<varchar>,

    PRIMARY KEY (nickname)
);

CREATE TABLE IF NOT EXISTS gamestore.games
(
    name          varchar,
    description   varchar,
    origin        varchar,
    genres        list<varchar>,
    developers    list<varchar>,
    release_date  timestamp,
    price_history map<timestamp, double>,

    PRIMARY KEY (name)
);
