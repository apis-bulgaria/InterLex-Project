/* USER CHANGE PASSWORD */
CREATE OR REPLACE FUNCTION user_change_password(
    _user_id integer,
    _password character varying)
  RETURNS void AS
$BODY$
BEGIN
UPDATE users SET password = md5(_password) WHERE user_id = _user_id;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE;

/*USER RESET PASSWORD */
CREATE OR REPLACE FUNCTION get_user_id_by_email(IN _email character varying)
  RETURNS TABLE(user_id integer, username character varying, email character varying, usertype_id integer, fullname character varying, push_session boolean, max_login_count integer, session_timeout integer, client_id integer, client_name character varying) AS
$BODY$
  DECLARE _user_id int;  
BEGIN
  _user_id := u.user_id FROM users u WHERE u.email = _email;
  RETURN QUERY
  SELECT u.user_id, u.username, u.email, u.usertype_id, u.fullname, u.push_session, u.max_login_count, u.session_timeout, c.client_id, c.client_name
  FROM users u JOIN clients c ON u.client_id = c.client_id WHERE u.user_id = _user_id;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_user_id_by_email(character varying)
  OWNER TO postgres;


/*GET LANGS */
CREATE OR REPLACE FUNCTION get_langs()
RETURNS setof langs AS $$
DECLARE r record;
BEGIN
FOR r IN SELECT * FROM langs 
LOOP
RETURN NEXT r;
END LOOP;
END;
$$
LANGUAGE plpgsql;

/*GET INTERFACE LANGS */
CREATE OR REPLACE FUNCTION get_interface_langs()
  RETURNS setof langs AS
$BODY$
DECLARE r record;
BEGIN
FOR R IN
  SELECT *
  FROM langs
  WHERE is_site_lang = true
  ORDER BY id
LOOP
RETURN NEXT r;
END LOOP;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE;

/*INITIAL USER LANGUAGE PREFERENCES -> CALLED BY TRIGGER ON USER INSERT*/

CREATE OR REPLACE FUNCTION insert_initial_user_lang_preferences()
RETURNS TRIGGER AS $$
DECLARE languages_count integer;
BEGIN
SELECT count(*) from langs WHERE is_site_lang = true INTO languages_count;
FOR i IN 1..languages_count
LOOP
INSERT INTO user_lang_preferences(user_id, lang, ord) VALUES (NEW.user_id, i, i);
END LOOP;
RETURN NEW;
END;
$$
LANGUAGE plpgsql;


/* USER UPDATE LANGUAGE PREFERENCES */
CREATE OR REPLACE FUNCTION update_user_lang_preferences(
    _user_id integer,
    _preferences integer[])
  RETURNS void AS
$BODY$
DECLARE
length integer := (SELECT count(*) FROM langs WHERE is_site_lang = true);
BEGIN
DELETE FROM user_lang_preferences WHERE user_id = _user_id;
FOR i IN 1..length
LOOP
INSERT INTO user_lang_preferences(user_id, lang, ord) VALUES (_user_id, _preferences[i][1], _preferences[i][2]);
/*UPDATE user_lang_preferences SET ord = _preferences[i][2] WHERE lang = _preferences[i][1] AND  user_id = _user_id ;*/
END LOOP;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION update_user_lang_preferences(integer, integer[])
  OWNER TO postgres;


/* GET USER LANG PREFERENCES BY ID*/
CREATE OR REPLACE FUNCTION get_user_lang_preferences(_user_id integer)
  RETURNS SETOF user_lang_preferences AS
$BODY$
DECLARE r record;
BEGIN
  FOR r in SELECT * FROM user_lang_preferences WHERE user_id = _user_id ORDER BY ord
  LOOP
    RETURN NEXT r;
  END LOOP;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_user_lang_preferences(integer)
  OWNER TO postgres;

/* GET SEARCH STATS */ 
CREATE OR REPLACE FUNCTION get_stat_searches()
  RETURNS SETOF stat_searches AS
$BODY$
DECLARE r record;
BEGIN
FOR r in SELECT id, txt, cnt FROM stat_searches ORDER BY cnt desc
LOOP
RETURN NEXT r;
END LOOP;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_stat_searches()
  OWNER TO postgres;

/*GET SEARCH LIKE STATS*/
CREATE OR REPLACE FUNCTION get_stat_searches(_input text, _take integer)
  RETURNS TABLE (txt character varying, cnt integer) AS
$BODY$
BEGIN
RETURN QUERY
SELECT s.txt, s.cnt FROM stat_searches s WHERE s.txt LIKE _input || '%' ORDER BY s.cnt desc LIMIT _take;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;


/*GET SEARCH STATS ADVANCED (FOR ADMINISTRATION)*/
CREATE OR REPLACE FUNCTION get_stat_searches(
    _input text,
    _take integer,
    _skip integer,
    _orderby text,
    _orderdir text)
  RETURNS SETOF stat_searches AS
$BODY$
DECLARE r record;
BEGIN
FOR r in SELECT id, txt, cnt FROM stat_searches WHERE txt LIKE _input || '%'
ORDER BY
CASE WHEN _orderby = 'txt' AND _orderdir = 'asc' THEN txt END,
 CASE WHEN _orderby = 'txt' AND _orderdir = 'desc' THEN txt END DESC,
CASE WHEN _orderby = 'cnt' AND _orderdir = 'asc' THEN to_char(cnt, '9G999') END,
CASE WHEN _orderby = 'cnt' AND _orderdir = 'desc' THEN to_char(cnt, '9G999') END DESC
LIMIT _take OFFSET _skip
LOOP
RETURN NEXT r;
END LOOP;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_stat_searches(text, integer, integer, text, text)
  OWNER TO postgres;

/*DELETE SEARCH STAT*/
CREATE OR REPLACE FUNCTION del_stat_search(_search_id integer)
RETURNS VOID AS
$$
BEGIN
DELETE FROM stat_searches WHERE id = _search_id;
END;
$$
LANGUAGE plpgsql;

/*GET TOTAL SEARCHES COUNT*/
CREATE OR REPLACE FUNCTION get_stat_total_searches_count()
RETURNS INTEGER AS
$$
BEGIN
RETURN sum(cnt) FROM stat_searches;
END;
$$
LANGUAGE plpgsql;

/*GET LIKE SEARCHES COUNT*/
CREATE OR REPLACE FUNCTION get_stat_like_searches_count(_like text)
RETURNS INTEGER AS
$$
BEGIN
RETURN count(*) FROM stat_searches WHERE txt LIKE _like || '%';
END;
$$
LANGUAGE plpgsql;

/*INSERT PASSWORD RESET*/
CREATE OR REPLACE FUNCTION add_password_reset(
    _user_id integer,
    _code text)
  RETURNS void AS
$BODY$
BEGIN
INSERT INTO password_resets(user_id, code, issue_date, expiry_date) 
VALUES (_user_id, _code, localtimestamp, null);
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;

/*GET PASSWORD RESET*/
CREATE OR REPLACE FUNCTION get_password_reset(IN _code text)
  RETURNS TABLE(_user_id integer, _issue_date timestamp without time zone, _expiry_date timestamp without time zone) AS
$BODY$
BEGIN
RETURN QUERY
SELECT user_id, issue_date, expiry_date FROM password_resets WHERE code = _code;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_password_reset(text)
  OWNER TO postgres;

/*UPDATE PASSWORD RESET EXPIRY DATE*/
CREATE OR REPLACE FUNCTION update_password_reset_expiry_date(_code text)
  RETURNS void AS
$BODY$
BEGIN
UPDATE password_resets SET expiry_date = localtimestamp WHERE code = _code;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;

/*ADD COOKIES AGREEMENT*/
CREATE OR REPLACE FUNCTION add_cookies_agreement(IN _user_id integer, IN _ip text)
RETURNS VOID AS
$$
BEGIN
INSERT INTO cookies_agreements(user_id, ip) VALUES (_user_id, _ip);
END;
$$
LANGUAGE plpgsql;

/*GET COOKIES AGREEMENT*/
CREATE OR REPLACE FUNCTION get_cookies_agreement(IN _user_id integer, IN _ip text)
RETURNS BOOLEAN AS
$$
DECLARE userIdCount integer;
DECLARE ipCount integer;
BEGIN
SELECT count(user_id) FROM cookies_agreements WHERE user_id = _user_id AND user_id IS NOT NULL INTO userIdCount;
SELECT count(ip) FROM cookies_agreements WHERE ip = _ip INTO ipCount;
IF userIdCount = 0 AND ipCount = 0 THEN RETURN FALSE;
ELSE RETURN TRUE;
END IF;
END;
$$
LANGUAGE plpgsql;

/*GET USER SEARCHES */
CREATE OR REPLACE FUNCTION get_user_searches(
    IN _user_id integer,
    IN _like text,
    IN _period integer,
    IN _type integer)
  RETURNS TABLE(id integer, txt character varying, search_date timestamp without time zone, details text) AS
$BODY$
BEGIN
  RETURN QUERY
  SELECT us.id, us.txt, us.search_date, us.search_obj::text
  FROM user_searches us
  WHERE us.user_id = _user_id AND us.txt LIKE _like || '%' AND
  CASE
	 WHEN _period = 0 THEN true
         WHEN _period = 1 AND us.search_date >= now()::date::timestamp THEN true -- today
         WHEN _period = 2 AND us.search_date < now()::date::timestamp AND us.search_date >= now()::date::timestamp - interval '1 day' THEN true -- yesterday
         WHEN _period = 3 AND us.search_date >= now() - interval '1 week' THEN true -- last week
         WHEN _period = 4 AND us.search_date >= now() - interval '1 month' THEN true -- last month
         ELSE false
  END AND
  CASE
	WHEN _type = 0 THEN true
	WHEN _type = 1 AND (us.search_obj->'Law'::text) = 'null' AND (us.search_obj->'Cases'::text) = 'null' THEN true --simple
	WHEN _type = 2 AND (us.search_obj->'Law'::text) != 'null' AND (us.search_obj->'Cases'::text) = 'null' THEN true --law
	WHEN _type = 3 AND (us.search_obj->'Law'::text) = 'null' AND (us.search_obj->'Cases'::text) != 'null' THEN true --cases
	ELSE false
  END
  ORDER BY us.search_date DESC;
END;
$BODY$
  LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_user_searches(
    IN _user_id integer,
    IN _like text,
    IN _period integer)
  RETURNS TABLE(id integer, txt character varying, search_date timestamp without time zone) AS
$$
BEGIN
  RETURN QUERY
  SELECT us.id, us.txt, us.search_date
  FROM user_searches us
  WHERE us.user_id = _user_id AND us.txt LIKE _like || '%' AND
  CASE
	 WHEN _period = 0 THEN true
         WHEN _period = 1 AND us.search_date >= now()::date::timestamp THEN true -- today
         WHEN _period = 2 AND us.search_date < now()::date::timestamp AND us.search_date >= now()::date::timestamp - interval '1 day' THEN true -- yesterday
         WHEN _period = 3 AND us.search_date >= now() - interval '1 week' THEN true -- last week
         WHEN _period = 4 AND us.search_date >= now() - interval '1 month' THEN true -- last month
         ELSE false
  END
  ORDER BY us.search_date DESC;
END;
$$LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_user_searches(integer, text, integer)
  OWNER TO postgres;


CREATE OR REPLACE FUNCTION get_user_searches(
    IN _user_id integer,
    IN _like text)
  RETURNS TABLE(id integer, txt character varying, search_date timestamp without time zone) AS
$$
BEGIN
  RETURN QUERY
  SELECT us.id, us.txt, us.search_date
  FROM user_searches us
  WHERE us.user_id = _user_id AND us.txt LIKE _like || '%' AND
  CASE
	 WHEN _period = 0 THEN true
         WHEN _period = 1 AND us.search_date >= now()::date::timestamp THEN true -- today
         WHEN _period = 2 AND us.search_date < now()::date::timestamp AND od.open_date >= now()::date::timestamp - interval '1 day' THEN true -- yesterday
         WHEN _period = 3 AND us.search_date >= now() - interval '1 week' THEN true -- last week
         WHEN _period = 4 AND us.search_date >= now() - interval '1 month' THEN true -- last month
         ELSE false
  END
  ORDER BY us.search_date DESC;
END;
$$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_user_searches(integer, text)
  OWNER TO postgres;

  CREATE OR REPLACE FUNCTION get_user_searches(IN _user_id integer)
  RETURNS TABLE(id integer, txt character varying, search_date timestamp without time zone) AS
$BODY$
BEGIN
  RETURN QUERY
  SELECT us.id, us.txt, us.search_date
  FROM user_searches us
  WHERE us.user_id = _user_id
  ORDER BY us.search_date DESC;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_user_searches(integer)
  OWNER TO postgres;

/*GET USER SEARCH DETAILS*/
CREATE OR REPLACE FUNCTION get_user_search_details(_search_id integer)
  RETURNS json AS
$BODY$
BEGIN
RETURN to_json(search_obj) FROM user_searches WHERE id = _search_id;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION get_user_search_details(integer)
  OWNER TO postgres;

/*DEL USER SEARCH*/
CREATE OR REPLACE FUNCTION del_user_search(_search_id integer)
  RETURNS void AS
$BODY$
BEGIN
DELETE FROM user_searches WHERE id = _search_id;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION del_user_search(integer)
  OWNER TO postgres;

/*GET DOC NUMBER BY DOC LANG ID*/
CREATE OR REPLACE FUNCTION get_doc_number_by_doc_lang_id(_doc_lang_id integer)
RETURNS TEXT AS
$$
BEGIN
RETURN doc_number FROM vw_doc_langs WHERE doc_lang_id = _doc_lang_id;
END;
$$
LANGUAGE PLPGSQL;

/*INSERT USER DOC*/
CREATE OR REPLACE FUNCTION add_user_doc(_user_id integer, _doc_lang_id integer)
RETURNS VOID AS
$$
BEGIN
INSERT INTO user_docs(user_id, doc_lang_id, add_date) VALUES (_user_id, _doc_lang_id, CURRENT_TIMESTAMP); 
END;
$$
LANGUAGE plpgsql;

/*DELETE USER DOC*/
CREATE OR REPLACE FUNCTION del_user_doc(_user_id integer, _doc_lang_id integer)
RETURNS VOID AS
$$
BEGIN
DELETE FROM user_docs WHERE user_id = _user_id AND doc_lang_id = _doc_lang_id;
END;
$$
LANGUAGE plpgsql;

/*GET USER DOCS COUNT*/
CREATE OR REPLACE FUNCTION get_user_docs_count(_user_id integer)
RETURNS INTEGER AS
$$
BEGIN
RETURN COUNT(*) FROM user_docs WHERE user_id = _user_id;
END;
$$
LANGUAGE plpgsql;

/*GET USER HAS DOCUMENT*/
CREATE OR REPLACE FUNCTION get_user_has_document(_user_id integer, _doc_lang_id integer)
RETURNS BOOLEAN AS
$$
BEGIN
IF (SELECT COUNT(*) FROM user_docs WHERE user_id = _user_id AND doc_lang_id = _doc_lang_id) = 0 THEN RETURN FALSE;
ELSE RETURN TRUE;
END IF;
END;
$$
LANGUAGE plpgsql;

/*GET USER FAVOURITE DOCS*/
CREATE OR REPLACE FUNCTION get_user_docs(
    IN _user_id integer,
    IN _site_lang_id integer)
  RETURNS TABLE(doc_lang_id integer, doc_type integer, lang_id integer, full_title character varying, doc_number character varying, keywords character varying[], summaries character varying[], country character varying, add_date timestamp without time zone) AS
$BODY$
BEGIN
RETURN QUERY
SELECT ud.doc_lang_id, d.doc_type, d.lang_id, d.full_title, d.doc_number,
(select array_agg_mult(ARRAY[ARRAY[source_name, kw.keywords]]) from get_doc_keywords(d.doc_lang_id, _site_lang_id, d.lang_id, _user_id) as kw) as keywords,
(select array_agg_mult(ARRAY[ARRAY[source_name, s.summary]]) from get_doc_summaries(d.doc_lang_id, _site_lang_id, d.lang_id, _user_id) as s) as summaries,
--array(select k.keyword::varchar from doc_keywords k where k.doc_lang_id = ud.doc_lang_id) as keywords,
d.country,
ud.add_date as add_date
FROM user_docs ud LEFT JOIN vw_doc_langs_mat d ON ud.doc_lang_id = d.doc_lang_id
WHERE ud.user_id = _user_id
ORDER BY ud.add_date DESC;
END;
$BODY$
LANGUAGE plpgsql;

  /*classifiers new*/

CREATE OR REPLACE FUNCTION get_classifier_base_id_by_type_id(_classifier_type_id integer)
RETURNS uuid
AS
$$
BEGIN
RETURN id FROM classifiers WHERE classifier_type_id = _classifier_type_id AND parent_id is null;
END;
$$
LANGUAGE PLPGSQL;
 
CREATE OR REPLACE FUNCTION get_classifiers_by_tree_level(IN _tree_level integer)
  RETURNS TABLE(_id uuid, _xml_id character varying, _classifier_type_id integer, _parent_id uuid, _langIds integer[], _langTitles text[]) AS
$BODY$
  BEGIN
  RETURN QUERY
  SELECT c.id, c.xml_id, c.classifier_type_id, c.parent_id,
  array(select cl.lang_id FROM classifier_langs cl WHERE classifier_id = c.id ORDER BY cl.id) as _langIds,
  array(select cl.title FROM classifier_langs cl WHERE classifier_id = c.id ORDER BY cl.id) as _langTitles 
  FROM classifiers c WHERE tree_level = _tree_level;
  END;
  $BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION get_classifiers_by_tree_level(integer)
  OWNER TO postgres;


  CREATE OR REPLACE FUNCTION get_classifiers_top_level_count()
  RETURNS INTEGER AS
  $$
  BEGIN 
  RETURN count(*) FROM classifiers WHERE tree_level = 1;
  END;
  $$
  LANGUAGE PLPGSQL;

  CREATE OR REPLACE FUNCTION get_classifiers_deepest_level()
  RETURNS integer AS
$BODY$
  BEGIN
  RETURN max(tree_level) FROM classifiers;
  END;
  $BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;

/*USER REQUESTS*/
CREATE OR REPLACE FUNCTION add_user_request(_requested_page text, _requested_on timestamp without time zone, _time_ms integer)
RETURNS VOID
AS
$$
BEGIN
INSERT INTO user_requests (requested_page, requested_on, time_ms) VALUES (_requested_page, _requested_on, _time_ms);
END;
$$
LANGUAGE PLPGSQL;

CREATE OR REPLACE FUNCTION get_user_requests(_order_by text, _order_dir text, take integer)
RETURNS TABLE (_requested_page text, _requested_on timestamp without time zone, _time_ms integer)
AS
$$
BEGIN
RETURN QUERY
SELECT requested_page, requested_on, time_ms FROM user_requests
ORDER BY
CASE WHEN _order_by = 'requested_page' AND _order_dir = 'asc' THEN requested_page END,
CASE WHEN _order_by = 'requested_page' AND _order_dir = 'desc' THEN requested_page END DESC,
CASE WHEN _order_by = 'requested_on' AND _order_dir = 'asc' THEN requested_on::text END,
CASE WHEN _order_by = 'requested_on' AND _order_dir = 'desc' THEN requested_on::text END DESC,
CASE WHEN _order_by = 'time_ms' AND _order_dir = 'asc' THEN to_char(time_ms, '9G999') END,
CASE WHEN _order_by = 'time_ms' AND _order_dir = 'desc' THEN to_char(time_ms, '9G999') END DESC
LIMIT take;
END;
$$
LANGUAGE PLPGSQL;

CREATE OR REPLACE FUNCTION get_document(IN _id integer, IN _user_id integer)
  RETURNS TABLE(doc_lang_id integer, lang_id integer, doc_type integer, title text, user_doc_id integer) AS
$BODY$
BEGIN
  RETURN QUERY
  SELECT vw.doc_lang_id, vw.lang_id, vw.doc_type, vw.full_title::text, ud.id as user_doc_id
  FROM vw_doc_langs vw LEFT JOIN user_docs ud ON _user_id = ud.user_id AND vw.doc_lang_id = ud.doc_lang_id
  WHERE vw.doc_lang_id = _id;
END;
$BODY$
  LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_users_count()
RETURNS INTEGER
AS 
$$
BEGIN
RETURN COUNT(*) FROM USERS;
END;
$$
LANGUAGE PLPGSQL;