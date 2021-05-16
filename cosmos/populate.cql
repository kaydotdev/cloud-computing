INSERT INTO gamestore.games (
name, description, origin,
genres, developers, release_date,
price_history
) VALUES (
    '',
    '',
    '',
    ['',''],
    [''],
    '1980-01-01',
    {
        '1980-01-01': 0.00
    }
);

SELECT app_id, name, description,
       origin, genres, developers,
       release_date, price_history
FROM gamestore.games WHERE app_id = 425580
ALLOW FILTERING;
