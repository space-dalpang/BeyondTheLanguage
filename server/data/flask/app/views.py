from flask import request, jsonify
from . import app, nlp, db
from .models import Word
import jpype


@app.route('/analyze', methods=['get'])
def analyze():
    jpype.attachThreadToJVM()

    query = request.args.get('q')
    posed = nlp.pos(query, norm=True, stem=True)
    return jsonify({"results": Word.pos_to_tag(posed)})


@app.route('/senti', methods=['get'])
def senti():
    jpype.attachThreadToJVM()

    query = request.args.get('q')
    posed = nlp.pos(query, norm=True, stem=True)
    tagged = Word.pos_to_tag(posed)

    filtered_words = db.session.query(Word).filter(
        db.literal(tagged).contains(Word.tag)
    )
    counted_words = list()
    senti_counter = dict()

    for word in filtered_words:
        count = tagged.count(word.tag)
        level_count = word.level * count
        if word.category in senti_counter:
            senti_counter[word.category] += level_count
        else:
            senti_counter[word.category] = level_count
        for i in range(count):
            counted_words.append(word)
    senti_counter['unmatched'] = len(posed) - len(counted_words)

    return jsonify({
        "analyzed": Word.pos_to_tag(posed),
        "words": [w.get_public() for w in counted_words],
        "senti": senti_counter
    })


@app.route('/words', methods=['get'])
def get_words():
    words = db.session.query(Word).all()
    return jsonify({"words": [w.get_public() for w in words]})


@app.route('/words', methods=['post'])
def post_words():
    jpype.attachThreadToJVM()

    json_parsed = request.get_json()
    words_json = json_parsed.get('words', [])

    db.session.query(Word).delete()

    for word_json in words_json:
        name = word_json.get('name')
        category = word_json.get('category')
        value = word_json.get('value')
        level = word_json.get('level')
        tag = word_json.get('tag')

        if tag is None:
            posed = nlp.pos(name, norm=True, stem=True)
            tag = Word.pos_to_tag(posed)

        if level is None:
            level = value // 3

        word = Word(
            name=name,
            category=category,
            level=level,
            tag=tag
        )

        db.session.add(word)

    db.session.commit()

    words = db.session.query(Word).all()

    return jsonify({"words": [w.get_public() for w in words]})
