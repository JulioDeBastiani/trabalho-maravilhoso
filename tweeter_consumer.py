import argparse
import tweepy
import struct

from datetime import datetime
from loguru import logger

TWEETS_FORMAT = '19s200s1120sdI'

parser = argparse.ArgumentParser()
parser.add_argument('-o', '--output-dir', default='data', help='output directory')
parser.add_argument('-f', '--from', required=False, help='timestamp from where to start the query')

ARGS = parser.parse_args()

if __name__ == "__main__":
    auth = tweepy.OAuthHandler('b0if3VKn7rIsqqzkVwoiPUR56', 'QiLjZOJ86EgQZ4FIPaQvVKjcJg2x9AlePMv8oRPW4YMu7ZuNj1')
    auth.set_access_token('1176636173277679616-sXGihslJSCwCOfTT1p9DLiG1dH6rqp', 'N63e1CLlNfOPxsIIN5CzGuMZIhgcFDQkUYWlj2rUrQIhf')
    api = tweepy.API(auth)

    search_words = '#gremio'

    tweets = tweepy.Cursor(api.search, q=search_words, lang='en').items(50)
    for tweet in tweets:
        logger.debug(tweet.id_str)
        logger.debug(tweet.user.name)
        logger.debug(tweet.text)
        logger.debug(tweet.created_at.timestamp())
        logger.debug(tweet.retweeted)
        logger.debug(tweet.retweet_count)

        rec = (
            tweet.id_str.encode('utf-8'),
            tweet.user.name.encode('utf-8'),
            tweet.text.encode('utf-8'),
            tweet.created_at.timestamp(),
            tweet.retweet_count
        )
        b = struct.pack(TWEETS_FORMAT, *rec)
        logger.debug(b)
        logger.debug(len(b))