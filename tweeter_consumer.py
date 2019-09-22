import argparse
import tweepy
import struct

from datetime import datetime
from loguru import logger

TWEETS_FORMAT = 'I200s1120s800sdI'

parser = argparse.ArgumentParser()
parser.add_argument('-o', '--output-dir', default='data', help='output directory')
parser.add_argument('-f', '--from', required=False, help='timestamp from where to start the query')

ARGS = parser.parse_args()

if __name__ == "__main__":
    rec = (1, 'test 1'.encode('utf-8'), 'aaadddd'.encode('utf-8'), 'test 3'.encode('utf-8'), datetime.utcnow().timestamp(), 5)
    b = struct.pack(TWEETS_FORMAT, *rec)
    logger.debug(b)
    logger.debug(len(b))