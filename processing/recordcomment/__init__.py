import json
import logging
from joblib import load
import azure.functions as func

import nltk
from nltk.stem import WordNetLemmatizer

nltk.download('wordnet')
nltk.download('punkt')

vectorizer = load('.models/comment_nlp_vectorizer.bin')
classifier = load('.models/comment_nlp_classifier.bin')
encoder = load('.models/comment_nlp_encoder.bin')


def predict_labels(features: str) -> list:
    tags = encoder.inverse_transform(
        classifier.predict(
            vectorizer.transform([features])
        )
    )

    return tags.tolist()[0]


def remove_punkt(word):
    for punkt in "?:!.,;'":
        word = word.replace(punkt, "")
    
    return word


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('HTTP comment recording requested.')

    try:
        req_body = req.get_json()
    except ValueError:
        return func.HttpResponse(
            json.dumps({
                "message": "Request body is missing."
            }),
            status_code=400
        )
    else:
        comment = req_body.get('comment')

    if comment is None:
        return func.HttpResponse(
            json.dumps({
                "message": "Required body parameters are missing."
            }),
            status_code=400
        )

    lemmatizer = WordNetLemmatizer()

    tokens = comment.split(" ")
    no_puckt = [remove_punkt(word) for word in tokens]
    words = [word.lower() for word in no_puckt]
    lemmas = [lemmatizer.lemmatize(word) for word in words]

    finalized_sentence = " ".join(lemmas)

    return func.HttpResponse(
        json.dumps({
            "reaction": predict_labels(finalized_sentence),
            "comment": comment
        }),
        status_code=200
    )
