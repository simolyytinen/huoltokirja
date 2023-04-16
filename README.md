## huoltokirja

Tämä projekti oli osa Savonia Code Academy 3 (SCA3) koodarikoulutusta ja se on tehty syksyllä 2022. Projekti on tehty kolmen hengen ryhmässä yhdessä VillePa (Ville Paasonen) ja Sinpaix (Sinna Oksman) kanssa.

## TO DO
- muokata sovellus käyttämään sqlite tietokantaa (saada sovellus toimimaan täysin lokaalisti eikä Azure pilvessä)
- siistiä / parantaa koodia
- siistiä / yhtenäistää frontendiä

## Yleistä
Harjoitustyössä vastattiin tarjouspyyntöön toteuttaa kunnossapitoyritykselle laitteiden kunnossapidon hallintaan soveltuva palvelu web-sovelluksena. Palvelussa piti pystyä kirjautumaan tavallisena käyttäjänä sekä admin-käyttäjänä. Käyttäjätunnusten salasanat tallennettiin tietokantaan salattuna. Vaatimuksena oli toteuttaa GRUD-toiminnot mm. huoltojen ja tarkastusten käsittelyyn, käyttäjätunnusten käsittelyyn, huoltokohteiden käsittelyyn sekä auditointien tekemiseen. Lisäksi huoltokohteille tuli pystyä tallentamaan tiedostoja (Azure Blob Storage). Projektin yksi merkittävä osa oli hyödyntää Azuren pilvipalveluita tietokannan (SQL) sekä dokumenttivaraston (Blob storage) ajamiseen. Mysö valmis sovellus (front ja backend) piti julkaista Azuren App Serviceä käyttäen. Koska projektin kehittämisen aikana käytettiin pilvessä olevia tietokantaa ja dokumenttivarastoa, ei sovellus tällä hetkellä toimi niiden puuttuessa. Minun on tarkoitus muuttaa sovellus käyttämään lokaalia SQLite tietokantaa, jotta voin kehittää sovellusta eteenppäin.

## Frontend
Projektin selainpään sovellus rakennettiin Blazorilla (ASP.NET) käyttäen C# ohjelmointikieltä.

## Backend


## Tietokanta
Tietokantana käytettiin projektin kehityksen aikana Azuren pilvessä pyörivää SQL tietokantaa. Kaikki siihen viittaavat yhteydet on poistettu nyt sovelluksesta.
