# ALLEGRO REST CLIENT
## Configuration
You need to put your api keys in **appsettings.json** and put it in to working directory

### ***appsettings.json*** SCHEMA
```json
{
  "API": {
    "SANDBOX": {
      "CLIENT_ID": "your-client-id-sandbox",
      "SECRET_ID": "your-secret-id-sandbox",
      "BASE_LINK": "https://allegro.pl.allegrosandbox.pl",
      "API_LINK": "https://api.allegro.pl.allegrosandbox.pl"
    },
    "PRODUCTION": {
      "CLIENT_ID": "your-client-id",
      "SECRET_ID": "your-secret-id",
      "BASE_LINK": "https://allegro.pl",
      "API_LINK": "https://api.allegro.pl"
    }

  }
}
```

To switch between Sandbox and Production just simply change flag **SANDBOX** in AllegroClient class