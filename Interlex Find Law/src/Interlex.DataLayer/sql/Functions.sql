
DROP AGGREGATE IF EXISTS array_agg_mult(anyarray);
CREATE AGGREGATE array_agg_mult (anyarray)  (
    SFUNC     = array_cat
   ,STYPE     = anyarray
   ,INITCOND  = '{}'
);

CREATE OR REPLACE FUNCTION search_court_acts(_user_id int, _site_search_id int) 
RETURNS int AS $$
BEGIN
  EXECUTE add_search(_user_id, _site_search_id);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_court_acts_search_list(_search_id int, _page int, _page_size int) 
RETURNS TABLE(doc_id int, title text) AS $$
DECLARE
  _sql varchar(2000);
  _table_name varchar(150);
  _offset int;
BEGIN
  _table_name := table_name FROM searches WHERE search_id = _search_id;
  _offset := (_page-1)*_page_size;
  _sql := --'SELECT d.doc_id, d.doc_identifier, d.doc_number, d.language, d.doc_type, d.full_title, d.publ_date 
          'SELECT d.doc_id, d.full_title as title
           FROM ' || _table_name || ' s JOIN documents d ON s.doc_id = d.doc_id 
           LIMIT ' || _page_size::varchar || ' OFFSET ' || _offset::varchar;
RAISE NOTICE '_sql: %', _sql;
  EXECUTE _sql;
END;
$$ LANGUAGE plpgsql;

-- get_lang
CREATE OR REPLACE FUNCTION get_lang(_id int) 
RETURNS TABLE(lang varchar, short_lang varchar, code varchar, name varchar) AS $$
BEGIN
  RETURN QUERY
  SELECT l.lang, l.short_lang, l.code, l.name
  FROM langs l
  WHERE id = _id;
END;
$$ LANGUAGE plpgsql;

-- get_document
CREATE OR REPLACE FUNCTION get_document(_id int) 
RETURNS TABLE(doc_lang_id int, lang_id int, doc_type int, title text) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.doc_lang_id, vw.lang_id, vw.doc_type, vw.full_title::text
  FROM vw_doc_langs vw
  WHERE vw.doc_lang_id = _id;
END;
$$ LANGUAGE plpgsql;

-- get_document_by_doc_number
CREATE OR REPLACE FUNCTION get_document_by_doc_number(_doc_number varchar, _lang_id int, _user_id int) 
RETURNS TABLE(doc_lang_id int, lang_id int, short_lang varchar, doc_type int, title text) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.doc_lang_id, vw.lang_id, l.short_lang, vw.doc_type, vw.full_title::text
  FROM vw_doc_langs_mat vw LEFT JOIN (SELECT _lang_id as lang_id, 0 as ord WHERE _lang_id IS NOT NULL
                            UNION
                            SELECT 4,1 WHERE _user_id IS NULL AND _lang_id <> 4
                            UNION
                            SELECT lang, ord FROM user_lang_preferences 
                            WHERE lang<>_lang_id AND user_id=_user_id) pref ON vw.lang_id = pref.lang_id
       JOIN langs l ON vw.lang_id = l.id
  WHERE lower(vw.doc_number) = lower(_doc_number)
  ORDER BY pref.ord
  LIMIT 1;
END;
$$ LANGUAGE plpgsql;

-- get_document_by_doc_identifier
CREATE OR REPLACE FUNCTION get_document_by_doc_identifier(_doc_identifier varchar) 
RETURNS TABLE(doc_lang_id int, lang_id int, short_lang varchar, doc_type int, title text) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.doc_lang_id, vw.lang_id, l.short_lang, vw.doc_type, vw.full_title::text
  FROM vw_doc_langs_mat vw JOIN langs l ON vw.lang_id = l.id
  WHERE lower(vw.doc_identifier) = lower(_doc_identifier);
END;
$$ LANGUAGE plpgsql;

-- get_search_ids_as_bytes
CREATE OR REPLACE FUNCTION get_search_ids_as_bytes(_search_id int) 
RETURNS SETOF bytea AS $$
BEGIN
  RETURN QUERY
  SELECT search_ids
  FROM searches
  WHERE search_id = _search_id;
END;
$$ LANGUAGE plpgsql;

-- get_search_table_name
CREATE OR REPLACE FUNCTION get_search_table_name(_user_id int, _session_id int, _site_search_id int) 
RETURNS varchar AS $$
BEGIN
  RETURN 'searches.search_u' || _user_id::varchar || '_sid' || _session_id::varchar || '_ssid' || _site_search_id || '_' || to_char(current_timestamp, 'YYYYMMDD_HH24MISS');
END;
$$ LANGUAGE plpgsql;

-- add_search
CREATE OR REPLACE FUNCTION add_search(_user_id int, _session_id int, _site_search_id int, _search_text_ids text, _site_lang_id int, _search_ids bytea) 
RETURNS TABLE(search_id int, total_count int) AS $$
DECLARE
  _search_id int;
  _sql text;
  _table_name varchar(150);
  _tmp_table_name varchar(150);
  _total_count int;
BEGIN
  _total_count := 0;
  SELECT get_search_table_name(_user_id, _session_id, _site_search_id) INTO _table_name;
  --_tmp_table_name := 'search_u' || _user_id::varchar || '_sid' || _session_id::varchar || '_ssid' || _site_search_id || '_' || to_char(current_timestamp, 'YYYYMMDD_HH24MISS') || '_tmp';
  
  INSERT INTO searches(session_id, site_search_id, table_name, search_date, search_ids) values(_session_id, _site_search_id, _table_name, now(), _search_ids) RETURNING searches.search_id INTO _search_id;
  
  _sql := 'CREATE UNLOGGED TABLE ' || _table_name || '(id serial, doc_lang_id int)'; -- WITH OIDS
  EXECUTE _sql;

  
  IF _search_text_ids IS NOT NULL THEN
    _sql := 'INSERT INTO ' || _table_name || '(doc_lang_id) VALUES' || _search_text_ids;
    EXECUTE _sql;

    GET DIAGNOSTICS _total_count = ROW_COUNT;

    --DROP TABLE tmp_all_ids;
    --EXECUTE _sql;
    --DROP TABLE tmp_lang_pref;
  END IF;

  RETURN QUERY
  SELECT _search_id, _total_count;
END;
$$ LANGUAGE plpgsql;

-- add_search_by_filter
-- CREATE OR REPLACE FUNCTION add_search_by_filter(_classifier_id uuid, _user_id int, _session_id int, _site_search_id int, _site_lang_id int) 
-- RETURNS TABLE(search_id int, total_count int) AS $$
-- DECLARE
--   _search_id int;
--   _sql text;
--   _table_name varchar(150);
--   _tmp_table_name varchar(150);
--   _total_count int;
-- BEGIN
--   _total_count := 0;
--   _table_name := FROM get_search_table_name(_user_id, _session_id, _site_search_id);
--   
--   INSERT INTO searches(session_id, site_search_id, table_name, search_date, search_ids) values(_session_id, _site_search_id, _table_name, now(), _search_ids) RETURNING searches.search_id INTO _search_id;
--   
--   _sql := 'CREATE UNLOGGED TABLE ' || _table_name || '(id serial, doc_lang_id int)'; -- WITH OIDS
--   EXECUTE _sql;
--   
--   _sql := 'WITH wRes AS (
--            SELECT doc_lang_id, ROW_NUMBER() OVER (PARTITION BY vw.doc_id ORDER BY pref.ord NULLS LAST) AS rn
--     FROM doc_classifiers dc JOIN vw_doc_langs_mat vw ON l.to_doc_lang_id = vw.doc_lang_id
--             LEFT JOIN (SELECT ' || _site_lang_id::varchar || ' as lang_id,0 as ord -- Most wanted language
--                              UNION 
--                              SELECT lang, ord FROM user_lang_preferences 
--                              WHERE lang<>' || _site_lang_id::varchar || ' AND user_id=' || _user_id::varchar || ') pref ON vw.lang_id = pref.lang_id
--     WHERE dc.classifier_id = ''' || _classifier_id::varchar || ''' )
--     INSERT INTO ' || _table_name || '(doc_lang_id) 
--     SELECT doc_lang_id FROM wRes';
--   EXECUTE _sql;
-- 
--   GET DIAGNOSTICS _total_count = ROW_COUNT;
-- 
--   RETURN QUERY
--   SELECT _search_id, _total_count;
-- END;
-- $$ LANGUAGE plpgsql;

-- get_class_parent_id_by_type_id
CREATE OR REPLACE FUNCTION get_class_parent_id_by_type_id(_class_filter_ids json, _type_id int) 
RETURNS uuid AS $$
  DECLARE _parent_id uuid;
BEGIN
  _parent_id := "Item2" FROM json_to_recordset(_class_filter_ids) AS x("Item1" int, "Item2" uuid) WHERE "Item1" = _type_id;
  RETURN _parent_id;
END;
$$ LANGUAGE plpgsql;

-- get_classifier_children_ids
CREATE OR REPLACE FUNCTION get_classifier_children_ids(_classifier_id uuid) 
RETURNS varchar[] AS $$
DECLARE _result varchar[];
BEGIN
  WITH RECURSIVE recursetree(id, parent_id) AS (
    SELECT id, parent_id FROM classifiers WHERE id = _classifier_id
  UNION ALL
    SELECT t.id, rt.parent_id FROM classifiers t JOIN recursetree rt ON rt.id = t.parent_id
  )
  SELECT array(SELECT id::varchar FROM recursetree) INTO _result;
  RETURN _result;
END;
$$ LANGUAGE plpgsql;

-- get_filter_classifier_types
CREATE OR REPLACE FUNCTION get_filter_classifier_types(_search_id int, _class_filter_ids json) 
RETURNS TABLE(classifier_id uuid, classifier_type_id int) AS $$
  DECLARE _sql varchar(2000);
  DECLARE _sql_where varchar;
  DECLARE _sql_filters varchar;
  DECLARE _parent_id varchar;
  DECLARE _search_table_name varchar(150);
  DECLARE _table_name_final varchar(150);
  DECLARE _doc_classifiers jsonb;
  DECLARE _r record;
BEGIN
  SELECT table_name, doc_classifiers INTO _search_table_name, _doc_classifiers FROM searches WHERE search_id = _search_id;
  _table_name_final := _search_table_name;
  
  _sql := '';
  FOR _r IN SELECT id FROM classifier_types
  LOOP
    _parent_id := get_class_parent_id_by_type_id(_class_filter_ids, _r.id);

    _sql_where := '';
    IF _parent_id IS NOT NULL THEN
      _sql_where := ' WHERE vw.parent_id = ''{' || _parent_id || '}''';
    END IF;
    
    _sql_filters := '';
    IF _class_filter_ids IS NOT NULL THEN
      _sql_filters := 'WITH w_res AS (
             SELECT s.doc_lang_id 
             FROM ' || _search_table_name || ' s 
             JOIN vw_doc_class_mat vw ON s.doc_lang_id = vw.doc_lang_id AND 
             vw.id IN (select "Item2" FROM json_to_recordset(''' || _class_filter_ids || ''') AS x("Item1" int, "Item2" uuid))
             GROUP BY s.doc_lang_id HAVING COUNT(*)=' || json_array_length(_class_filter_ids)::varchar || '
           ) ';
      _table_name_final := 'w_res';
    END IF;
    
    IF _sql != '' THEN
      _sql := _sql || ' UNION ';
    END IF;  
    _sql := _sql || ' (SELECT vw.id, vw.classifier_type_id 
             FROM ' || _table_name_final || ' s 
             JOIN vw_doc_class_mat vw ON s.doc_lang_id = vw.doc_lang_id AND vw.classifier_type_id=' || _r.id::varchar || '
             ' || _sql_where || '
             LIMIT 1)';
  END LOOP;

  _sql := _sql_filters || _sql;
  
raise notice '_sql=%', _sql;
  RETURN QUERY
  EXECUTE _sql;         
END;
$$ LANGUAGE plpgsql;

-- get_filter_classifiers
CREATE OR REPLACE FUNCTION get_filter_classifiers(_selected_classifiers uuid[], _lang_id int, _doc_classifiers jsonb)
RETURNS TABLE(classifier_id uuid, classifier_type_id int, title text, cnt int) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.id, vw.classifier_type_id, vw.title, x.c
  FROM vw_classifiers vw JOIN jsonb_to_recordset(_doc_classifiers) AS x("c" int, "i" uuid) ON vw.id = x.i
  WHERE vw.parent_id IN (
                         SELECT c.id
                         FROM classifiers c 
                         WHERE c.tree_level = 1 AND 
                               c.classifier_type_id NOT IN (SELECT c.classifier_type_id FROM classifiers c JOIN unnest(_selected_classifiers) as class_id ON c.id=class_id)
                         UNION
                         SELECT unnest(_selected_classifiers)
                         ) AND 
        vw.lang_id = _lang_id
  ORDER BY vw.classifier_type_id, vw.ord;
  
END;
$$ LANGUAGE plpgsql;

-- get_filter_classifier
CREATE OR REPLACE FUNCTION get_filter_classifier(_search_id int, _parent_id uuid, _classifier_type_id int, _lang_id int, _class_filter_ids uuid[]) 
RETURNS TABLE(classifier_id uuid, title text, cnt int) AS $$
  DECLARE _sql varchar;
  DECLARE _sql_filters varchar;
  DECLARE _search_table_name varchar(150);
BEGIN
  _search_table_name := table_name FROM searches WHERE search_id = _search_id;

  IF _parent_id IS NULL THEN
    _parent_id := id FROM classifiers WHERE parent_id IS NULL AND classifier_type_id = _classifier_type_id;
  END IF;
  
  _sql_filters := '';
  IF _class_filter_ids IS NOT NULL THEN
    _sql_filters := 'WITH wRes AS (
             SELECT s.doc_lang_id 
             FROM ' || _search_table_name || ' s JOIN vw_doc_class_mat vw ON s.doc_lang_id = vw.doc_lang_id AND 
                                                                             vw.id IN (''' || array_to_string(_class_filter_ids, ''',''') || ''')
             GROUP BY s.doc_lang_id HAVING COUNT(*)=' || array_length(_class_filter_ids,1)::varchar || '
           )';
    _search_table_name := 'wRes';
  END IF;
  
  -- _sql := _sql_filters || ' SELECT vw.id, cl.title, COUNT(*)::int
--            FROM ' || _search_table_name || ' s
--            JOIN vw_doc_class_mat vw ON s.doc_lang_id = vw.doc_lang_id AND vw.parent_id = ''' || _parent_id::varchar || '''
--            JOIN classifier_langs cl ON cl.classifier_id = vw.id AND cl.lang_id = ' || _lang_id::varchar || '
--            GROUP BY vw.id, cl.title, vw.ord
--            ORDER BY vw.ord';

  _sql := _sql_filters || ' SELECT vw.id, cl.title, COUNT(*)::int
           FROM ' || _search_table_name || ' s
           JOIN vw_doc_class_mat vw ON s.doc_lang_id = vw.doc_lang_id AND vw.parent_id = ''' || _parent_id::varchar || '''
           JOIN classifier_langs cl ON cl.classifier_id = vw.id AND cl.lang_id = ' || _lang_id::varchar || '
           GROUP BY vw.id, cl.title, vw.ord
           ORDER BY vw.ord';

  raise notice '_sql=%', _sql;
  RETURN QUERY
  EXECUTE _sql;         
END;
$$ LANGUAGE plpgsql;

-- get_search_doc_lang_ids
CREATE OR REPLACE FUNCTION get_search_doc_lang_ids(_search_id int) 
RETURNS int[] AS $$
  DECLARE _sql varchar;
  DECLARE _search_table_name varchar(150);
  DECLARE _ids int[];
BEGIN
  _search_table_name := table_name FROM searches WHERE search_id = _search_id;

  _sql := 'SELECT array(SELECT doc_lang_id FROM ' || _search_table_name || ')';
  
raise notice '_sql=%',_sql;
  EXECUTE _sql INTO _ids;

  RETURN _ids;
END;
$$ LANGUAGE plpgsql;

-- get_doc_keywords
CREATE OR REPLACE FUNCTION get_doc_keywords(_doc_lang_id int, _site_lang_id int, _lang_id int, _user_id int) 
RETURNS TABLE(source_name varchar, keywords varchar) AS $$
BEGIN
  RETURN QUERY
  WITH res AS (
    SELECT di.name as source_name, string_agg(keyword, ', ')::varchar as keywords, di.ord,
           ROW_NUMBER() OVER (PARTITION BY di.name ORDER BY pref.ord NULLS LAST) as rn
    FROM doc_keywords dk LEFT JOIN (SELECT _site_lang_id as lang_id,0 as ord -- Most wanted language
                                    UNION
                                    SELECT _lang_id, 0
                                    UNION 
                                    SELECT lang, ord FROM user_lang_preferences 
                                    WHERE lang not in (_site_lang_id, _lang_id) AND user_id=_user_id
                                    ) pref ON dk.lang_id = pref.lang_id
                              JOIN doc_info_sources di ON dk.source_id = di.id
    WHERE dk.doc_lang_id = _doc_lang_id
    GROUP BY di.name, di.ord, pref.ord
  )
  SELECT res.source_name, res.keywords FROM res WHERE res.rn=1 ORDER BY res.ord;
END;
$$ LANGUAGE plpgsql;

-- get_doc_summaries
CREATE OR REPLACE FUNCTION get_doc_summaries(_doc_lang_id int, _site_lang_id int, _lang_id int,  _user_id int) 
RETURNS TABLE(source_name varchar, summary varchar) AS $$
BEGIN
  RETURN QUERY
  WITH res AS (
    SELECT di.name as source_name, ds.summary as summary, 
           ROW_NUMBER() OVER (PARTITION BY di.name ORDER BY pref.ord NULLS LAST) as rn
    FROM doc_summaries ds LEFT JOIN (SELECT _site_lang_id as lang_id,-1 as ord -- Most wanted language
                                    UNION
                                    SELECT _lang_id, 0
                                    UNION 
                                    SELECT lang, ord FROM user_lang_preferences 
                                    WHERE lang not in (_site_lang_id, _lang_id) AND user_id=_user_id
                                    ) pref ON ds.lang_id = pref.lang_id
                              JOIN doc_info_sources di ON ds.source_id = di.id
    WHERE ds.doc_lang_id = _doc_lang_id
  )
  SELECT res.source_name, res.summary FROM res WHERE rn=1;
END;
$$ LANGUAGE plpgsql;

-- get_search_list_count
CREATE OR REPLACE FUNCTION get_search_list_count(_search_id int, _class_filter_ids uuid[]) 
RETURNS int AS $$
  DECLARE _sql varchar;
  DECLARE _search_table_name varchar(150);
  DECLARE _total_count int;
BEGIN
  _search_table_name := table_name FROM searches WHERE search_id = _search_id;

  IF _class_filter_ids IS NOT NULL THEN
    _sql := 'SELECT COUNT(1) FROM (
             SELECT COUNT(1) as cnt
             FROM ' || _search_table_name || ' t JOIN doc_classifiers dc ON t.doc_lang_id = dc.doc_lang_id
             WHERE dc.classifier_id IN (''' || array_to_string(_class_filter_ids, ''',''') || ''')
             GROUP BY t.id, t.doc_lang_id HAVING COUNT(*)=' || array_length(_class_filter_ids,1)::varchar || ') t';
  ELSE
    _sql := 'SELECT COUNT(1) as cnt FROM ' || _search_table_name;
  END IF;
raise notice '_sql=%',_sql;
  EXECUTE _sql INTO _total_count;

  RETURN _total_count;
END;
$$ LANGUAGE plpgsql;

-- get_search_list
CREATE OR REPLACE FUNCTION get_search_list(
    IN _search_id integer,
    IN _class_filter_ids uuid[],
    IN _sort character varying,
    IN _sort_dir character varying,
    IN _page integer,
    IN _page_size integer,
    IN _user_id integer,
    IN _site_lang_id integer)
  RETURNS TABLE(doc_lang_id integer, lang_id integer, doc_number varchar, doc_type_id integer, country varchar(5), title text, keywords character varying[][], summaries character varying[], user_doc_id integer) AS
$BODY$
  DECLARE _sql varchar;
  DECLARE _offset int;
  DECLARE _search_table_name varchar(150);
  DECLARE _search_table_name_final varchar(150);
BEGIN
  _search_table_name := table_name FROM searches WHERE search_id = _search_id;
  _search_table_name_final := _search_table_name;
  IF _sort = 'title' THEN _search_table_name_final := _search_table_name || '_title'; END IF;
  IF _sort = 'date'  THEN _search_table_name_final := _search_table_name || '_date'; END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                 WHERE  n.nspname = 'searches' AND c.relname = replace(_search_table_name_final, 'searches.', '')) THEN
    _sql := 'CREATE UNLOGGED TABLE ' || _search_table_name_final || ' (LIKE ' || _search_table_name || ')';
    EXECUTE _sql;

    _sql := '';
    IF _sort = 'title' THEN
      _sql := 'INSERT INTO ' || _search_table_name_final || '(id, doc_lang_id) 
               SELECT t.id, s.doc_lang_id FROM ' || _search_table_name || ' s JOIN _sort_titles t ON s.doc_lang_id=t.doc_lang_id 
               ORDER BY t.id';
    ELSE IF _sort = 'date' THEN
      _sql := _sql || 'WITH wRes AS (
                         SELECT s.doc_lang_id, ROW_NUMBER() OVER(ORDER BY vw.publ_date) as rn
                         FROM ' || _search_table_name || ' s JOIN vw_doc_langs_2_mat vw ON s.doc_lang_id=vw.doc_lang_id
                       )
                       INSERT INTO ' || _search_table_name_final || '(id, doc_lang_id)
                       SELECT rn, doc_lang_id FROM wRes';
      END IF;
    END IF;
    
    IF _sql <> '' THEN
      EXECUTE _sql;
    END IF;
  END IF;
  
  _offset := (_page-1)*_page_size;

  IF _class_filter_ids IS NOT NULL THEN
    _sql := 'WITH page AS (
             SELECT t.doc_lang_id
             FROM ' || _search_table_name_final || ' t JOIN doc_classifiers dc ON t.doc_lang_id = dc.doc_lang_id
             WHERE dc.classifier_id IN (''' || array_to_string(_class_filter_ids, ''',''') || ''')
             GROUP BY t.id, t.doc_lang_id HAVING COUNT(*)=' || array_length(_class_filter_ids,1)::varchar || '
             ORDER BY t.id ' || _sort_dir || '
             LIMIT ' || _page_size::varchar || ' OFFSET ' || _offset::varchar || ')';
  ELSE
    _sql := 'WITH page AS (
             SELECT t.doc_lang_id
             FROM ' || _search_table_name_final || ' t ORDER BY t.id ' || _sort_dir || '
             LIMIT ' || _page_size::varchar || ' OFFSET ' || _offset::varchar || ')';
  END IF;
  _sql := _sql || ' SELECT dl.doc_lang_id, dl.lang_id, dl.doc_number, dl.doc_type, dl.country, dl.full_title::text, 
       (select array_agg_mult(ARRAY[ARRAY[source_name, keywords]]) from get_doc_keywords(dl.doc_lang_id, '||_site_lang_id::varchar||', dl.lang_id, '||_user_id::varchar||')) as keywords,
       (select array_agg_mult(ARRAY[ARRAY[source_name, summary]]) from get_doc_summaries(dl.doc_lang_id, '||_site_lang_id::varchar||', dl.lang_id, '||_user_id::varchar||')) as summaries,
       ud.id as user_doc_id
FROM page s JOIN vw_doc_langs_mat dl ON s.doc_lang_id = dl.doc_lang_id 
       LEFT JOIN user_docs ud ON ' || _user_id || '= ud.user_id AND dl.doc_lang_id = ud.doc_lang_id';
raise notice '_sql=%', _sql;
  RETURN QUERY
  EXECUTE _sql;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;

-- get_user_lang_pref_for_search
CREATE OR REPLACE FUNCTION get_user_lang_pref_for_search(_user_id int, _site_lang_id int) 
RETURNS int[] AS $$  
DECLARE
  _res int[];
BEGIN
  _res := array(select lang from user_lang_preferences where user_id=_user_id and lang<>_site_lang_id order by ord);
  return array_cat(array[_site_lang_id], _res);
END;
$$ LANGUAGE plpgsql;

-- get_user_id_by_username_password
CREATE OR REPLACE FUNCTION get_user_id_by_username_password(_username varchar(100), _password varchar(100)) 
RETURNS TABLE(user_id int, username varchar(100), email varchar, usertype_id int, fullname varchar(100), push_session bool, max_login_count int, session_timeout int, client_id int, client_name varchar(500)) AS $$
  DECLARE _user_id int;  
BEGIN
  _user_id := u.user_id FROM users u WHERE u.username = _username AND u.password = md5(_password);
  RETURN QUERY
  SELECT u.user_id, u.username, u.email, u.usertype_id, u.fullname, u.push_session, u.max_login_count, u.session_timeout, c.client_id, c.client_name
  FROM users u JOIN clients c ON u.client_id = c.client_id WHERE u.user_id = _user_id;
END;
$$ LANGUAGE plpgsql;

-- get_user
CREATE OR REPLACE FUNCTION get_user(IN _user_id integer)
  RETURNS TABLE(user_id integer, username character varying, email character varying, usertype_id integer, fullname character varying, push_session boolean, max_login_count integer, session_timeout integer, client_id integer, client_name character varying) AS
$BODY$
BEGIN
  RETURN QUERY
  SELECT u.user_id, u.username, u.email, u.usertype_id, u.fullname, u.push_session, u.max_login_count, u.session_timeout, c.client_id, c.client_name
  FROM users u JOIN clients c ON u.client_id = c.client_id WHERE u.user_id = _user_id;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;

-- clear_old_sessions
-- CREATE OR REPLACE FUNCTION clear_old_sessions(_timeout_sec int) 
-- RETURNS void AS $$
-- BEGIN
--   DELETE FROM sessions WHERE EXTRACT(EPOCH FROM now()-last_access) > _timeout_sec;
--   --EXECUTE 'DELETE FROM sessions WHERE last_access < now() - interval ''' || _timeout_sec::varchar || ' seconds''';
-- END;
-- $$ LANGUAGE plpgsql;

-- clear_old_sessions
CREATE OR REPLACE FUNCTION clear_old_sessions() 
RETURNS void AS $$
BEGIN
  DELETE FROM sessions s WHERE EXTRACT(EPOCH FROM now()-last_access) > (SELECT session_timeout*60 FROM users u WHERE u.user_id=s.user_id);
  
  --DELETE FROM sessions WHERE EXTRACT(EPOCH FROM now()-last_access) > _timeout_sec;
  --EXECUTE 'DELETE FROM sessions WHERE last_access < now() - interval ''' || _timeout_sec::varchar || ' seconds''';
END;
$$ LANGUAGE plpgsql;

-- del_session
CREATE OR REPLACE FUNCTION del_session(_session_id int) 
RETURNS void AS $$
BEGIN
  DELETE FROM sessions WHERE session_id = _session_id;
END;
$$ LANGUAGE plpgsql;

-- add_session
-- RETURNS: > 0 new session_id; -1: max login count reached; -2: subscription expired
CREATE OR REPLACE FUNCTION add_session(_user_id int, _ip_addr varchar(100)) 
RETURNS int AS $$
  DECLARE _session_id int; 
  DECLARE _active_sessions_cnt int;
  DECLARE _max_login_count int;
  DECLARE _push_session bool;
  DECLARE _del_session_id int;
BEGIN
  _session_id := -1;
  IF NOT EXISTS(SELECT up.id
                FROM user_products up JOIN users u ON up.user_id = u.user_id
                                      JOIN client_products cp ON cp.client_id = u.client_id AND cp.product_id = up.product_id
                WHERE up.user_id = _user_id AND cp.start_date <= now() AND cp.end_date >= now()) THEN
    _session_id := -2; -- subscription expired
  ELSE
    _active_sessions_cnt := COUNT(*) FROM sessions WHERE user_id = _user_id;
    SELECT max_login_count, push_session INTO _max_login_count, _push_session FROM users WHERE user_id = _user_id;
    _session_id := -1; -- max login count reached
    IF _active_sessions_cnt < _max_login_count OR _push_session = true THEN
      IF _active_sessions_cnt >= _max_login_count THEN
        _del_session_id := s.session_id FROM sessions s WHERE s.user_id = _user_id ORDER BY s.last_access DESC LIMIT 1;
        EXECUTE del_session(_del_session_id);
      END IF;
      INSERT INTO sessions(user_id, start_date, last_access, ip_addr) VALUES(_user_id, now(), now(), _ip_addr) RETURNING session_id INTO _session_id;
    END IF;
  END IF;

  RETURN _session_id;
END;
$$ LANGUAGE plpgsql;

-- update_session_last_access
CREATE OR REPLACE FUNCTION update_session_last_access(_session_id int) 
RETURNS void AS $$
BEGIN
  UPDATE sessions SET last_access = now() WHERE session_id = _session_id;
END;
$$ LANGUAGE plpgsql;

-- update_session
CREATE OR REPLACE FUNCTION update_session(_session_id int, _timeoutSeconds int) 
RETURNS bool AS $$
BEGIN
  IF EXISTS(SELECT * FROM sessions WHERE session_id = _session_id AND EXTRACT(EPOCH FROM now()-last_access) <= _timeoutSeconds) THEN
    EXECUTE update_session_last_access(_session_id);
    RETURN true;
  ELSE
    EXECUTE del_session(_session_id);
    RETURN false;
  END IF;
END;
$$ LANGUAGE plpgsql;

-- get_sessions
CREATE OR REPLACE FUNCTION get_sessions(_search_text varchar(100), _start_from int, _page_size int, _sort_by varchar(30), _sort_dir varchar(4)) 
RETURNS TABLE(session_id int, start_date timestamp, last_access timestamp, ip_addr varchar(100), user_id int, username varchar(100), 
              usertype varchar(100), client_name varchar(500)) AS $$
BEGIN
  RETURN QUERY
  SELECT s.session_id, s.start_date, s.last_access, s.ip_addr,
         u.user_id, u.username, ut.sysname, c.client_name
  FROM sessions s JOIN users u ON s.user_id = u.user_id 
                           JOIN clients c ON u.client_id = c.client_id
                           JOIN usertypes ut ON u.usertype_id = ut.usertype_id
  WHERE (_search_text IS NULL OR u.username ILIKE '%'||_search_text||'%' OR c.client_name ILIKE '%'||_search_text||'%')
  ORDER BY 
    CASE WHEN _sort_by = 'last_access' AND _sort_dir = 'asc' THEN s.last_access END,
    CASE WHEN _sort_by = 'last_access' AND _sort_dir = 'desc' THEN s.last_access END DESC,
    CASE WHEN _sort_by = 'username' AND _sort_dir = 'asc' THEN u.username END,
    CASE WHEN _sort_by = 'username' AND _sort_dir = 'desc' THEN u.username END DESC,
    CASE WHEN _sort_by = 'client_name' AND _sort_dir = 'asc' THEN c.client_name  END,
    CASE WHEN _sort_by = 'client_name' AND _sort_dir = 'desc' THEN c.client_name END DESC
  LIMIT _page_size OFFSET _start_from;
END;
$$ LANGUAGE plpgsql;

-- get_users
CREATE OR REPLACE FUNCTION get_users(_search_text varchar(100), _start_from int, _page_size int, _sort_by varchar(30), _sort_dir varchar(4)) 
RETURNS TABLE(user_id int, username varchar(100), push_session bool, max_login_count int, session_timeout int, 
              usertype varchar(100), fullname varchar(100), client_id int, client_name varchar(500)) AS $$
BEGIN
  RETURN QUERY
  SELECT u.user_id, u.username, u.push_session, u.max_login_count, u.session_timeout, ut.sysname, u.fullname, c.client_id, c.client_name
  FROM users u JOIN clients c ON u.client_id = c.client_id
               JOIN usertypes ut ON u.usertype_id = ut.usertype_id
  WHERE (_search_text IS NULL OR u.username ILIKE '%'||_search_text||'%' OR u.fullname ILIKE '%'||_search_text||'%' OR c.client_name ILIKE '%'||_search_text||'%')
  ORDER BY 
    CASE WHEN _sort_by = 'username' AND _sort_dir = 'asc' THEN u.username END,
    CASE WHEN _sort_by = 'username' AND _sort_dir = 'desc' THEN u.username END DESC,
    CASE WHEN _sort_by = 'fullname' AND _sort_dir = 'asc' THEN u.fullname END,
    CASE WHEN _sort_by = 'fullname' AND _sort_dir = 'desc' THEN u.fullname END DESC,
    CASE WHEN _sort_by = 'client_name' AND _sort_dir = 'asc' THEN c.client_name  END,
    CASE WHEN _sort_by = 'client_name' AND _sort_dir = 'desc' THEN c.client_name END DESC
  LIMIT _page_size OFFSET _start_from;
END;
$$ LANGUAGE plpgsql;

-- del_user
CREATE OR REPLACE FUNCTION del_user(_user_id int) 
RETURNS void AS $$
BEGIN
  DELETE FROM users WHERE user_id = _user_id;
END;
$$ LANGUAGE plpgsql;

-- del_client
CREATE OR REPLACE FUNCTION del_client(_client_id int) 
RETURNS void AS $$
BEGIN
  DELETE FROM users WHERE client_id = _client_id;
  DELETE FROM client_products WHERE client_id = _client_id;
  DELETE FROM clients WHERE client_id = _client_id;
END;
$$ LANGUAGE plpgsql;

-- get_usertypes
CREATE OR REPLACE FUNCTION get_usertypes() 
RETURNS TABLE(usertype_id int, name varchar(100)) AS $$
BEGIN
  RETURN QUERY
  SELECT ut.usertype_id, ut.en_name FROM usertypes ut ORDER BY usertype_id;
END;
$$ LANGUAGE plpgsql;

-- get_clients
CREATE OR REPLACE FUNCTION get_clients() 
RETURNS TABLE(client_id int, client_name varchar(100)) AS $$
BEGIN
  RETURN QUERY
  SELECT c.client_id, c.client_name FROM clients c ORDER BY c.client_name;
END;
$$ LANGUAGE plpgsql;

-- get_clients
CREATE OR REPLACE FUNCTION get_clients(_search_text varchar(100), _start_from int, _page_size int, _sort_by varchar(30), _sort_dir varchar(4)) 
RETURNS TABLE(client_id int, client_name varchar(500)) AS $$
BEGIN
  RETURN QUERY
  SELECT c.client_id, c.client_name
  FROM clients c
  WHERE (_search_text IS NULL OR c.client_name ILIKE '%'||_search_text||'%')
  ORDER BY 
    CASE WHEN _sort_by = 'client_name' AND _sort_dir = 'asc' THEN c.client_name  END,
    CASE WHEN _sort_by = 'client_name' AND _sort_dir = 'desc' THEN c.client_name END DESC
  LIMIT _page_size OFFSET _start_from;
END;
$$ LANGUAGE plpgsql;

-- set_user
CREATE OR REPLACE FUNCTION set_user(
    _user_id integer,
    _client_id integer,
    _username character varying,
    _email character varying,
    _password character varying,
    _usertype_id integer,
    _fullname character varying,
    _push_sess boolean,
    _max_login_count integer,
    _session_timeout integer,
    _prods integer[])
  RETURNS integer AS
$$
DECLARE
  _product_id int;
  _license_cnt int;
BEGIN
  IF EXISTS(SELECT * FROM users WHERE user_id = _user_id) THEN
    UPDATE users SET client_id = _client_id,
                     username = _username,
                     email = _email,
                     password = case when _password is null then password else md5(_password) end,
                     usertype_id = _usertype_id,
                     fullname = _fullname,
                     push_session = _push_sess,
                     max_login_count = _max_login_count,
                     session_timeout = _session_timeout
    WHERE user_id = _user_id;
  ELSE
    INSERT INTO users(client_id, username, email, password, usertype_id, fullname, push_session, max_login_count, session_timeout) 
    VALUES(_client_id, _username, _email, md5(_password), _usertype_id, _fullname, _push_sess, _max_login_count, _session_timeout)
    RETURNING users.user_id INTO _user_id;
  END IF;

  -- set user products
  FOR i IN 1..array_length(_prods, 1) LOOP
    _product_id := _prods[i][1];
    _license_cnt := _prods[i][2];
    IF EXISTS(SELECT * FROM user_products WHERE user_id = _user_id AND product_id = _product_id) THEN
      UPDATE user_products SET license_cnt = _license_cnt
      WHERE user_id = _user_id AND product_id = _product_id;
    ELSE
      INSERT INTO user_products(user_id, product_id, license_cnt) VALUES(_user_id, _product_id, _license_cnt);
    END IF;
  END LOOP;

  RETURN _user_id;
END;
$$ LANGUAGE plpgsql;

-- set_client
CREATE OR REPLACE FUNCTION set_client(_client_id int, _client_name varchar(500), _prod_ids int[], _prod_lic int[], _prod_dates timestamp[][]) 
RETURNS int AS $$
BEGIN
  IF EXISTS(SELECT * FROM clients WHERE client_id = _client_id) THEN
    UPDATE clients SET client_name = _client_name
    WHERE client_id = _client_id;

    DELETE FROM client_products 
    WHERE client_id = _client_id AND product_id NOT IN (SELECT * FROM unnest(_prod_ids));
  ELSE
    INSERT INTO clients(client_name) VALUES(_client_name)
    RETURNING clients.client_id INTO _client_id;
    
  END IF;
  -- set products
  FOR i IN 1..array_length(_prod_ids, 1) LOOP
    IF EXISTS(SELECT * FROM client_products WHERE client_id = _client_id AND product_id = _prod_ids[i]) THEN
      UPDATE client_products SET license_cnt = _prod_lic[i], start_date = _prod_dates[i][1], end_date = _prod_dates[i][2]
      WHERE client_id = _client_id AND product_id = _prod_ids[i];
    ELSE
      INSERT INTO client_products(client_id, product_id, license_cnt, start_date, end_date)
      SELECT _client_id, _prod_ids[i], _prod_lic[i], _prod_dates[i][1], _prod_dates[i][2];
    END IF;
  END LOOP;

  RETURN _client_id;
END;
$$ LANGUAGE plpgsql;

-- get_client
CREATE OR REPLACE FUNCTION get_client(_client_id int)
RETURNS TABLE(client_id int, client_name varchar(500)) AS $$
BEGIN
  RETURN QUERY
  SELECT c.client_id, c.client_name
  FROM clients c
  WHERE c.client_id = _client_id;
END;
$$ LANGUAGE plpgsql;

-- get_client_products
CREATE OR REPLACE FUNCTION get_client_products(_client_id int)
RETURNS TABLE(product_id int, product_name varchar(100), license_cnt int, start_date timestamp, end_date timestamp) AS $$
BEGIN
  RETURN QUERY
  SELECT p.product_id, p.product_name, cp.license_cnt, cp.start_date, cp.end_date
  FROM client_products cp JOIN products p ON cp.product_id = p.product_id
  WHERE cp.client_id = _client_id
  ORDER BY p.product_id;
END;
$$ LANGUAGE plpgsql;

-- get_products
CREATE OR REPLACE FUNCTION get_products()
RETURNS TABLE(product_id int, product_name varchar(100)) AS $$
BEGIN
  RETURN QUERY
  SELECT p.product_id, p.product_name
  FROM products p
  ORDER BY p.product_id;
END;
$$ LANGUAGE plpgsql;

-- get_user_products
CREATE OR REPLACE FUNCTION get_user_products(_user_id int)
RETURNS TABLE(product_id int, product_name varchar(100), license_cnt int) AS $$
BEGIN
  RETURN QUERY
  SELECT p.product_id, p.product_name, up.license_cnt
  FROM user_products up JOIN products p ON up.product_id = p.product_id
  WHERE up.user_id = _user_id
  ORDER BY p.product_id;
END;
$$ LANGUAGE plpgsql;

-- get_products
CREATE OR REPLACE FUNCTION get_products(_client_id int)
RETURNS TABLE(product_id int, product_name varchar(100), license_cnt int, start_date timestamp, end_date timestamp, selected bool) AS $$
BEGIN
  RETURN QUERY
  SELECT p.product_id, p.product_name, cp.license_cnt, cp.start_date, cp.end_date, 
         CASE WHEN cp.client_id IS NULL THEN false ELSE true END
  FROM products p LEFT JOIN client_products cp ON p.product_id = cp.product_id AND cp.client_id = _client_id
  ORDER BY p.product_id;
END;
$$ LANGUAGE plpgsql;

-- add_recent_doc
CREATE OR REPLACE FUNCTION add_recent_doc(_user_id int, _doc_lang_id int, _max_count int) 
RETURNS int AS $$
DECLARE
  _count int;
BEGIN
  DELETE FROM opened_docs 
  WHERE id IN (
    SELECT id FROM opened_docs WHERE user_id = _user_id
    ORDER BY pinned DESC, open_date DESC
    OFFSET _max_count
  );

  IF EXISTS(SELECT * FROM opened_docs WHERE user_id = _user_id AND doc_lang_id = _doc_lang_id) THEN
    UPDATE opened_docs SET open_date = now() WHERE user_id = _user_id AND doc_lang_id = _doc_lang_id;
  ELSE -- add doc
    _count := COUNT(*) FROM opened_docs WHERE user_id = _user_id;

    IF _count = _max_count THEN
      -- try to delete last unpinned doc if max count is reached. If max count is reached and all docs are pinned the new doc will be not added.
      DELETE FROM opened_docs 
      WHERE id IN (
        SELECT id FROM opened_docs WHERE user_id = _user_id AND pinned = false
        ORDER BY open_date
        LIMIT 1
      );
    END IF;
    _count := COUNT(*) FROM opened_docs WHERE user_id = _user_id;
    IF _count < _max_count THEN
      INSERT INTO opened_docs(user_id, doc_lang_id, pinned, open_date) VALUES(_user_id, _doc_lang_id, false, now());
    END IF;
  END IF;

  RETURN 0;
END;
$$ LANGUAGE plpgsql;

-- get_recent_docs
CREATE OR REPLACE FUNCTION get_recent_docs(
    IN _user_id integer,
    IN _site_lang_id integer,
    IN _pinned boolean,
    IN _doc_type integer,
    IN _period integer,
    IN _order_by character varying,
    IN _sort_dir character varying)
  RETURNS TABLE(id integer, doc_lang_id integer, country varchar(5), doc_type integer, full_title text, doc_number varchar, keywords varchar[], summaries varchar[], pinned boolean, open_date timestamp without time zone, user_doc_id integer) AS
$BODY$
BEGIN
  RETURN QUERY
  SELECT od.id, vw.doc_lang_id, vw.country, vw.doc_type, vw.full_title::text, vw.doc_number,
  --array(select k.keyword::varchar from doc_keywords k where k.doc_lang_id = od.doc_lang_id) as keywords,
  (select array_agg_mult(ARRAY[ARRAY[source_name, kw.keywords]]) from get_doc_keywords(vw.doc_lang_id, _site_lang_id, vw.lang_id, _user_id) as kw) as keywords,
  (select array_agg_mult(ARRAY[ARRAY[source_name, s.summary]]) from get_doc_summaries(vw.doc_lang_id, _site_lang_id, vw.lang_id, _user_id) as s) as summaries,
   od.pinned, od.open_date,
   ud.id as user_doc_id
  FROM opened_docs od JOIN vw_doc_langs vw ON od.doc_lang_id = vw.doc_lang_id 
                 LEFT JOIN user_docs ud ON _user_id = ud.user_id AND od.doc_lang_id = ud.doc_lang_id
  
  WHERE od.user_id = _user_id AND 
       (_pinned IS NULL OR od.pinned = _pinned) AND 
       (_doc_type IS NULL OR vw.doc_type = _doc_type) AND
       CASE 
         WHEN _period = 0 THEN true
         WHEN _period = 1 AND od.open_date >= now()::date::timestamp THEN true -- today
         WHEN _period = 2 AND od.open_date < now()::date::timestamp AND od.open_date >= now()::date::timestamp - interval '1 day' THEN true -- yesterday
         WHEN _period = 3 AND od.open_date >= now() - interval '1 week' THEN true -- last week
         WHEN _period = 4 AND od.open_date >= now() - interval '1 month' THEN true -- last month
         ELSE false
       END
  ORDER BY pinned DESC, -- always pinned first
           CASE WHEN _order_by = 'title' AND _sort_dir = 'asc' THEN vw.full_title END,
           CASE WHEN _order_by = 'title' AND _sort_dir = 'desc' THEN vw.full_title END DESC,
           CASE WHEN _order_by = 'open_date' AND _sort_dir = 'asc' THEN od.open_date END,
           CASE WHEN _order_by = 'open_date' AND _sort_dir = 'desc' THEN od.open_date END DESC;
      
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;

-- set_recent_doc_pin
CREATE OR REPLACE FUNCTION set_recent_doc_pin(_id int, _pinned boolean)
RETURNS void AS $$
BEGIN
  UPDATE opened_docs SET pinned = _pinned WHERE id = _id;
END;
$$ LANGUAGE plpgsql;

-- get_new_docs
CREATE OR REPLACE FUNCTION get_new_docs(_user_id int, _site_lang_id int)
RETURNS TABLE(doc_lang_id integer, country varchar(5), doc_type integer, full_title text, doc_number varchar, keywords varchar[], summaries varchar[], user_doc_id integer) AS $$
BEGIN
  RETURN QUERY
  WITH wRes AS (
  SELECT vw.doc_lang_id, vw.country, vw.doc_type, vw.full_title::text, vw.doc_number,
  (select array_agg_mult(ARRAY[ARRAY[source_name, kw.keywords]]) from get_doc_keywords(vw.doc_lang_id, _site_lang_id, vw.lang_id, _user_id) as kw) as keywords,
  (select array_agg_mult(ARRAY[ARRAY[source_name, s.summary]]) from get_doc_summaries(vw.doc_lang_id, _site_lang_id, vw.lang_id, _user_id) as s) as summaries,
  ud.id as user_doc_id,
  ROW_NUMBER() OVER (PARTITION BY vw.doc_id ORDER BY pref.ord NULLS LAST) as rn
  FROM _new_docs n JOIN vw_doc_langs_mat vw ON n.doc_lang_id = vw.doc_lang_id
              LEFT JOIN user_docs ud ON _user_id = ud.user_id AND vw.doc_lang_id = ud.doc_lang_id
              LEFT JOIN (SELECT _site_lang_id as lang_id,0 as ord -- Most wanted language
                                    UNION
                                    SELECT lang, ord FROM user_lang_preferences 
                                    WHERE lang not in (_site_lang_id) AND user_id=_user_id
                                    ) pref ON vw.lang_id = pref.lang_id
  )
  SELECT r.doc_lang_id, r.country,r.doc_type, r.full_title, r.doc_number, r.keywords, r.summaries, r.user_doc_id 
  FROM wRes r
  WHERE r.rn = 1;
  
END;
$$ LANGUAGE plpgsql;

-- username_exists
CREATE OR REPLACE FUNCTION username_exists(_user_id int, _username varchar(100))
RETURNS boolean AS $$
BEGIN
  IF EXISTS(SELECT * FROM users WHERE (_user_id IS NULL OR user_id<>_user_id) AND username = _username) THEN
    RETURN true;
  ELSE
    RETURN false;
  END IF;
END;
$$ LANGUAGE plpgsql;

-- get_doc_text
CREATE OR REPLACE FUNCTION get_doc_text(_doc_lang_id int, _plain_xml boolean)
RETURNS text AS $$
DECLARE
  _s            varchar;
  _p            varchar;
  _pid          integer;
  _in_link      boolean;
BEGIN
  SELECT INTO _s '';
  FOR _p, _pid, _in_link IN SELECT t.content, p.id,
                      CASE 
                        WHEN _plain_xml IS true THEN false
                        ELSE EXISTS(SELECT * FROM vw_doc_links_mat vw WHERE vw.to_doc_lang_id = _doc_lang_id AND vw.to_doc_par_id = p.id) 
                      END
                      FROM doc_pars p JOIN par_texts t ON p.par_text_id = t.id WHERE p.doc_lang_id = _doc_lang_id ORDER BY p.ord
  LOOP
    SELECT INTO _s _s || CASE
                           WHEN NOT _in_link THEN
                             regexp_replace(_p, '^(\s*?<\w+?\s)', '\1' || 'pid="' || _pid::varchar || '" ')
                           ELSE 
                             regexp_replace(_p, '^(\s*?<\w+?\s)', '\1' || 'in_link="' || _pid::varchar || '" pid="' || _pid::varchar || '" ')
                         END;
  END LOOP;
  RETURN _s;
END;
$$ LANGUAGE plpgsql;

-- get_doc_par_text
CREATE OR REPLACE FUNCTION get_doc_par_text(_doc_number text, _to_par text, _lang_id_from_doc int, _user_id int, _site_lang_id int)
RETURNS TABLE(doc_lang_id int, doc_type int, lang_id int, country varchar, title varchar, par_text text) AS $$
  DECLARE _doc_lang_id int;
  DECLARE _lang_code varchar;
  DECLARE _par_id int;
  DECLARE _doc_type int;
  DECLARE _lang_id int;
  DECLARE _content text;
  DECLARE _title varchar;
  DECLARE _country varchar;
BEGIN
  CREATE TEMP TABLE lang_prefs(
    lang_id int,
    ord int primary key
  );

  INSERT INTO lang_prefs(lang_id, ord)
  SELECT _lang_id_from_doc as lang_id, -1 as ord WHERE _lang_id_from_doc IS NOT NULL
  UNION
  SELECT _site_lang_id,0 
  UNION 
  SELECT 4, 1 WHERE _user_id IS NULL -- Get english if _site_lang_id not found. Used in ananymous webapi
  UNION
  SELECT lang, ord+3 FROM user_lang_preferences WHERE lang<>_site_lang_id AND user_id=_user_id;

  SELECT l.to_doc_lang_id, l.to_doc_par_id INTO _doc_lang_id, _par_id
  FROM links l LEFT JOIN lang_prefs pref ON l.lang_id = pref.lang_id
  WHERE lower(l.celex) = lower(_doc_number) AND (_to_par IS NULL OR lower(to_par) = lower(_to_par))
  ORDER BY pref.ord LIMIT 1;

  IF _doc_lang_id IS NOT NULL THEN

    SELECT vw.doc_type, vw.lang_id, vw.country INTO _doc_type, _lang_id, _country FROM vw_doc_langs_mat vw WHERE vw.doc_lang_id = _doc_lang_id;

    -- if _to_par is not found or is null in the params show document start
    IF _to_par IS NULL OR _par_id IS NULL THEN
      _title := null; -- title is in the doc_text xml, not needed here
      _content = get_doc_text(_doc_lang_id, true);
    ELSE
      SELECT vw.content INTO _content 
      FROM vw_doc_text vw 
      WHERE vw.doc_lang_id = _doc_lang_id AND (coalesce(_par_id,-1) = -1 OR vw.doc_par_id = _par_id)
      LIMIT 1;

      raise notice '_content=%',_content;
                  
      _title := full_title FROM doc_langs WHERE id = _doc_lang_id;
    END IF;
  END IF;

  raise notice '_doc_lang_id=%',_doc_lang_id;
  raise notice '_par_id=%',_par_id;

  IF _content IS NULL THEN
    _lang_code := l.short_lang FROM langs l LEFT JOIN lang_prefs pref ON l.id = pref.lang_id ORDER BY pref.ord LIMIT 1;
    _country := 'EU';
    _content := 'http://eur-lex.europa.eu/legal-content/' || _lang_code || '/ALL/?uri=CELEX:' || _doc_number;
    _content := '<p><a href="' || _content || '" target="_blank">' || _content || '</a></p>';
  END IF;

  DROP TABLE lang_prefs;
  
  RETURN QUERY
  SELECT _doc_lang_id, _doc_type, _lang_id, _country, _title, _content;
END;
$$ LANGUAGE plpgsql;

-- get_doc_contents
CREATE OR REPLACE FUNCTION get_doc_contents(_doc_lang_id int) 
RETURNS TABLE(doc_par_id int, parent_doc_par_id int, eid varchar, num varchar, heading varchar) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.doc_par_id, CASE WHEN vw2.eid IS NULL THEN null ELSE vw.parent_doc_par_id END, vw.eid, vw.num, vw.heading
  FROM vw_doc_text vw LEFT JOIN vw_doc_text vw2 ON vw.parent_doc_par_id = vw2.doc_par_id
  WHERE vw.doc_lang_id = _doc_lang_id AND vw.eid IS NOT NULL AND (vw.num IS NOT NULL OR vw.heading IS NOT NULL)
  ORDER BY vw.ord;
END;
$$ LANGUAGE plpgsql;

-- get_doc_langs
CREATE OR REPLACE FUNCTION get_doc_langs(_doc_lang_id int) 
RETURNS TABLE(doc_lang_id int, lang_id int) AS $$
  DECLARE _version_id int;
BEGIN
  _version_id := vw.version_id FROM vw_doc_langs_mat vw where vw.doc_lang_id=_doc_lang_id;
  
  RETURN QUERY
  select vw.doc_lang_id, vw.lang_id from vw_doc_langs_2_mat vw join langs l on vw.lang_id=l.id where version_id=_version_id order by l.ord;
END;
$$ LANGUAGE plpgsql;

-- get_doc_list_by_ids
CREATE OR REPLACE FUNCTION get_doc_list_by_ids(_doc_lang_ids int[], _filter_domain varchar)
RETURNS TABLE(doc_lang_id int, doc_type int, title text, original_link text) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.doc_lang_id, vw.doc_type, vw.full_title::text, source_url::text
  FROM unnest(_doc_lang_ids) id JOIN vw_doc_langs_mat vw ON id = vw.doc_lang_id
  WHERE (
    (lower(_filter_domain) = 'all') OR 
    (lower(_filter_domain) = 'eul' AND vw.doc_type = 2 AND vw.country = 'EU') OR
    (lower(_filter_domain) = 'eucl' AND vw.doc_type = 1 AND vw.country = 'EU') OR
    (lower(_filter_domain) = 'natl' AND vw.doc_type = 2 AND vw.country <> 'EU') OR
    (lower(_filter_domain) = 'natcl' AND vw.doc_type = 1 AND vw.country <> 'EU')
  );
END;
$$ LANGUAGE plpgsql;

-- get_link_info
CREATE OR REPLACE FUNCTION get_link_info(_doc_number varchar, _to_par varchar, _user_id int, _site_lang_id int)
RETURNS TABLE(doc_lang_id int, doc_type int, title text, doc_par_id int, par_title varchar) AS $$
BEGIN
  RETURN QUERY
  WITH wRes AS (
    SELECT l.to_doc_lang_id, vw.doc_type, vw.full_title::text, l.to_doc_par_id, t.num as par_title,
           ROW_NUMBER() OVER (PARTITION BY vw.doc_id ORDER BY pref.ord NULLS LAST) AS rn
    FROM links l JOIN vw_doc_langs_mat vw ON l.to_doc_lang_id = vw.doc_lang_id
            LEFT JOIN vw_doc_text t ON t.doc_lang_id = vw.doc_lang_id AND t.doc_par_id = l.to_doc_par_id
            LEFT JOIN (SELECT _site_lang_id as lang_id,0 as ord -- Most wanted language
                       UNION 
                       SELECT 4, 1 WHERE _user_id IS NULL -- Get english if _site_lang_id not found. Used in ananymous webapi
                       UNION
                       SELECT lang, ord FROM user_lang_preferences 
                       WHERE lang not in (_site_lang_id) AND user_id=_user_id) pref ON vw.lang_id = pref.lang_id
    WHERE lower(l.celex) = lower(_doc_number) AND (_to_par IS NULL OR lower(l.to_par) = lower(_to_par)) 
    LIMIT 1        
  )
  SELECT to_doc_lang_id, r.doc_type, r.full_title, 
         CASE WHEN _to_par IS NULL THEN null ELSE r.to_doc_par_id END, 
         CASE WHEN _to_par IS NULL THEN null ELSE r.par_title END
  --INTO _doc_lang_id, _par_id 
  FROM wRes r WHERE rn = 1;

--raise notice '_doc_lang_id=%',_doc_lang_id;
--raise notice '_par_id=%',_par_id;

END;
$$ LANGUAGE plpgsql;

-- get_doc_in_links
CREATE OR REPLACE FUNCTION get_doc_in_links(_doc_number varchar, _to_par varchar, _filter_domain varchar, _user_id int, _site_lang_id int, _limit int)
RETURNS TABLE(doc_lang_id int, doc_type int, title text, original_link text, total_count bigint) AS $$
DECLARE _doc_lang_id int;
DECLARE _par_id int;
BEGIN
  -- SELECT f.doc_lang_id INTO _doc_lang_id FROM get_document_by_doc_number(_doc_number, _site_lang_id, _user_id) f;
--   IF _doc_lang_id IS NOT NULL AND _to_par IS NOT NULL THEN
--     SELECT vw.doc_par_id INTO _par_id FROM vw_doc_text vw WHERE vw.doc_lang_id = _doc_lang_id AND vw.eid = _to_par;
--     raise notice '_to_par=%',_to_par;
--     IF _par_id IS NULL THEN
--       RETURN;
--     END IF;
--   END IF;
  

  RETURN QUERY
  SELECT f.* FROM get_doc_in_links(_doc_lang_id, _par_id, _filter_domain, _user_id, _site_lang_id, _limit) f;
END;
$$ LANGUAGE plpgsql;

-- get_doc_versions
CREATE OR REPLACE FUNCTION get_doc_versions(_doc_lang_id int, _lang_id int, _site_lang_id int, _user_id int)
RETURNS TABLE(doc_number varchar, doc_lang_id int, lang_id int, doc_date timestamp) AS $$
DECLARE _doc_id int;
BEGIN
  _doc_id := vw.doc_id FROM vw_doc_langs_2_mat vw WHERE vw.doc_lang_id = _doc_lang_id;
  RETURN QUERY
  WITH wRes AS (
  SELECT vw.doc_number, vw.doc_lang_id, vw.lang_id, vw.doc_date, ROW_NUMBER() OVER (PARTITION BY vw.version_id ORDER BY pref.ord NULLS LAST) AS rn
  FROM vw_doc_langs_mat vw LEFT JOIN (SELECT _lang_id as lang_id,-1 as ord -- Most wanted language
                             UNION 
                             SELECT _site_lang_id, 0
                             UNION
                             SELECT lang, ord FROM user_lang_preferences 
                             WHERE lang not in (_site_lang_id, _lang_id) AND user_id=_user_id) pref ON vw.lang_id = pref.lang_id
  WHERE vw.doc_id = _doc_id
  )
  SELECT r.doc_number, r.doc_lang_id, r.lang_id, r.doc_date FROM wRes r WHERE rn = 1 ORDER BY r.doc_date DESC;
END;
$$ LANGUAGE plpgsql;

-- get_eid_from_topar
CREATE OR REPLACE FUNCTION get_eid_from_topar(_doc_lang_id int, _to_par varchar)
RETURNS varchar AS $$
DECLARE _doc_par_id int;
DECLARE _eid varchar;
BEGIN
  SELECT to_doc_par_id INTO _doc_par_id FROM links l WHERE l.to_doc_lang_id = _doc_lang_id AND lower(l.to_par) = lower(_to_par) LIMIT 1;
  raise notice '_doc_par_id=%',_doc_par_id;
  SELECT eid INTO _eid FROM vw_doc_text WHERE doc_par_id = _doc_par_id;
  raise notice '_eid=%',_eid;
  RETURN _eid;
END;
$$ LANGUAGE plpgsql;

-- get_doc_in_links
CREATE OR REPLACE FUNCTION get_doc_in_links(_doc_lang_id int, _to_par_id int, _filter_domain varchar, _user_id int, _site_lang_id int, _limit int)
RETURNS TABLE(doc_lang_id int, doc_type int, title text, original_link text, total_count bigint) AS $$
BEGIN
  RETURN QUERY
  WITH wRes AS (
    SELECT dl.doc_lang_id, ROW_NUMBER() OVER (PARTITION BY dl.doc_id ORDER BY pref.ord NULLS LAST) AS rn
    FROM vw_doc_links_mat vw JOIN vw_doc_langs_mat dl ON vw.doc_lang_id = dl.doc_lang_id
                        LEFT JOIN (SELECT _site_lang_id as lang_id,0 as ord -- Most wanted language
                             UNION 
                             SELECT 4, 1 WHERE _user_id IS NULL -- Get english if _site_lang_id not found. Used in ananymous webapi
                             UNION
                             SELECT lang, ord FROM user_lang_preferences 
                             WHERE lang<>_site_lang_id AND user_id=_user_id) pref ON vw.lang_id = pref.lang_id
  WHERE vw.to_doc_lang_id = _doc_lang_id AND (_to_par_id IS NULL OR vw.to_doc_par_id = _to_par_id)
  )
  SELECT vw.doc_lang_id, vw.doc_type, vw.full_title::text, source_url::text, count(*) OVER() AS total_count
  FROM wRes res JOIN vw_doc_langs_mat vw ON res.doc_lang_id = vw.doc_lang_id
  WHERE res.rn = 1 AND 
        (
	    (lower(_filter_domain) = 'all') OR 
	    (lower(_filter_domain) = 'eul' AND vw.doc_type = 2 AND vw.country = 'EU') OR
	    (lower(_filter_domain) = 'eucl' AND vw.doc_type = 1 AND vw.country = 'EU') OR
	    (lower(_filter_domain) = 'natl' AND vw.doc_type = 2 AND vw.country <> 'EU') OR
	    (lower(_filter_domain) = 'natcl' AND vw.doc_type = 1 AND vw.country <> 'EU')
        )
  LIMIT _limit;
END;
$$ LANGUAGE plpgsql;

-- site version
-- get_doc_in_links
CREATE OR REPLACE FUNCTION get_doc_in_links(_doc_lang_id int, _to_par_id int)
RETURNS TABLE(doc_lang_id int) AS $$
BEGIN
  RETURN QUERY
  SELECT DISTINCT vw.doc_lang_id FROM vw_doc_links_mat vw 
  WHERE vw.to_doc_lang_id = _doc_lang_id AND (_to_par_id IS NULL OR vw.to_doc_par_id = _to_par_id);
  
    -- SELECT dl.doc_lang_id
--     FROM vw_doc_links_mat vw JOIN vw_doc_langs_mat dl ON vw.doc_lang_id = dl.doc_lang_id
--     WHERE vw.to_doc_lang_id = _doc_lang_id AND (_to_par_id IS NULL OR vw.to_doc_par_id = _to_par_id) 
--         AND (
-- 	    (lower(_filter_domain) = 'all') OR 
-- 	    (lower(_filter_domain) = 'eul' AND dl.doc_type = 2 AND dl.country = 'EU') OR
-- 	    (lower(_filter_domain) = 'eucl' AND dl.doc_type = 1 AND dl.country = 'EU') OR
-- 	    (lower(_filter_domain) = 'natl' AND dl.doc_type = 2 AND dl.country <> 'EU') OR
-- 	    (lower(_filter_domain) = 'natcl' AND dl.doc_type = 1 AND dl.country <> 'EU')
--         )
END;
$$ LANGUAGE plpgsql;

-- get_doc_links_title
CREATE OR REPLACE FUNCTION get_doc_links_title(_doc_lang_id int, _to_par_id int)
RETURNS TABLE(doc_number varchar, par_title varchar) AS $$
DECLARE 
  _doc_number varchar; 
  _par_title varchar;
BEGIN
  
  SELECT vw.doc_number INTO _doc_number FROM vw_doc_langs_mat vw WHERE vw.doc_lang_id = _doc_lang_id;

  IF _to_par_id IS NOT NULL THEN
    SELECT num INTO _par_title FROM vw_doc_text WHERE doc_par_id = _to_par_id;
  END IF;

  RETURN QUERY
  SELECT _doc_number, _par_title;
END;
$$ LANGUAGE plpgsql;

-- get_classifier_type_id_by_name
CREATE OR REPLACE FUNCTION get_classifier_type_id_by_name(_name text)
RETURNS int AS $$
BEGIN
  RETURN id from classifier_types where lower(classifier_type) = lower(_name);
END;
$$ LANGUAGE plpgsql;

-- get_classifier
CREATE OR REPLACE FUNCTION get_classifier(_classifier_type_id int, _parent_id uuid, _lang_id int)
RETURNS TABLE(id uuid, name text, parent_id uuid, ord int) AS $$
BEGIN
  RETURN QUERY
  SELECT c.id, cl.title, c.parent_id, c.ord
  FROM classifiers c JOIN classifier_langs cl ON c.id = cl.classifier_id AND cl.lang_id = _lang_id
  WHERE c.classifier_type_id = _classifier_type_id AND ((_parent_id is null AND c.parent_id IS NULL) OR c.parent_id = _parent_id)
  ORDER BY c.ord;
END;
$$ LANGUAGE plpgsql;

-- get_classifier_keypathlist
CREATE OR REPLACE FUNCTION get_classifier_keypathlist(_id uuid, _skip_root boolean)
RETURNS varchar AS $$
DECLARE
  _path varchar;
BEGIN
  _path := '';

  while _id is not null and (_skip_root = false or exists(select * from classifiers where id = _id and parent_id is not null)) loop
    _path := _id::varchar || '/' || _path;
    _id := parent_id from classifiers where id = _id;
  end loop;

  RETURN '/' || trim(trailing '/' from  _path);
END;
$$ LANGUAGE plpgsql;

-- get_classifier_keypathlist_title
CREATE OR REPLACE FUNCTION get_classifier_keypathlist_title(_id uuid, _lang_id int, _skip_root boolean)
RETURNS varchar AS $$
DECLARE
  _title varchar;
BEGIN
  _title := '';

  while _id is not null and (_skip_root = false or exists(select * from classifiers where id = _id and parent_id is not null)) loop
    _title := (select title from classifiers c join classifier_langs cl on c.id = cl.classifier_id 
               where c.id = _id and cl.lang_id = _lang_id) || ' / ' || _title;
    _id := parent_id from classifiers where id = _id;
  end loop;

  _title := trim(both ' ' from  _title);
  RETURN trim(both '/' from  _title);
END;
$$ LANGUAGE plpgsql;

-- set_stat_search
CREATE OR REPLACE FUNCTION set_stat_search(_txt varchar)
RETURNS int AS $$
DECLARE
  _stat_search_id int;
BEGIN
  IF EXISTS(SELECT * FROM stat_searches WHERE txt = lower(_txt)) THEN
    UPDATE stat_searches SET cnt = cnt+1 WHERE txt = lower(_txt) RETURNING id INTO _stat_search_id;
  ELSE
    INSERT INTO stat_searches(txt, cnt) VALUES(lower(_txt), 1) RETURNING id INTO _stat_search_id;
  END IF;

  RETURN _stat_search_id;
END;
$$ LANGUAGE plpgsql;

-- set_stat_search_doc
CREATE OR REPLACE FUNCTION set_stat_search_doc(_stat_search_id int, _doc_id int)
RETURNS void AS $$
BEGIN
  IF EXISTS(SELECT * FROM stat_search_docs WHERE stat_search_id = _stat_search_id AND doc_id = _doc_id) THEN
    UPDATE stat_search_docs SET cnt = cnt+1 WHERE stat_search_id = _stat_search_id AND doc_id = _doc_id;
  ELSE
    INSERT INTO stat_search_docs(stat_search_id, doc_id, cnt) VALUES(_stat_search_id, _doc_id, 1);
  END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_user_search(
    _user_id integer,
    _txt character varying,
    _search_obj jsonb,
    _max_user_search_count integer)
  RETURNS void AS $$
  DECLARE current_search_count integer;
  DECLARE oldest_entry_id integer;
BEGIN
  SELECT COUNT(*) FROM user_searches WHERE user_id = _user_id INTO current_search_count;
  IF current_search_count + 1 > _max_user_search_count THEN 
    SELECT id INTO oldest_entry_id FROM user_searches WHERE user_id = _user_id order by search_date LIMIT 1;
    DELETE FROM user_searches WHERE id = oldest_entry_id;
  END IF;
  INSERT INTO user_searches(user_id, txt, search_obj, search_date) VALUES(_user_id, _txt, _search_obj, now());
END;
$$ LANGUAGE plpgsql;

-- get_user_searches
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

-- get_folders
CREATE OR REPLACE FUNCTION get_folders(_product_id int, _site_lang_id int) 
RETURNS TABLE(folder_id int, parent_folder_id int, name varchar, query_type int, query varchar) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.id, vw.parent_id, vw.name, vw.query_type, vw.query
  FROM vw_folders vw
  WHERE vw.product_id = _product_id and vw.lang_id = _site_lang_id
  ORDER BY vw.ord, vw.name;
END;
$$ LANGUAGE plpgsql;

-- get_folder
CREATE OR REPLACE FUNCTION get_folder(_folder_id int) 
RETURNS TABLE(folder_id int, parent_folder_id int, query_type int, query varchar) AS $$
BEGIN
  RETURN QUERY
  SELECT f.id, f.parent_id, f.query_type, f.query
  FROM folders f
  WHERE f.id = _folder_id;
END;
$$ LANGUAGE plpgsql;

-- get_folder_langs
CREATE OR REPLACE FUNCTION get_folder_langs(_folder_id int) 
RETURNS TABLE(lang_id int, name varchar, title_path varchar) AS $$
BEGIN
  RETURN QUERY
  SELECT vw.lang_id, vw.name, get_folder_path_title(vw.id, vw.lang_id, false)
  FROM vw_folders vw
  WHERE vw.id = _folder_id;
END;
$$ LANGUAGE plpgsql;

-- get_folder_path_title
CREATE OR REPLACE FUNCTION get_folder_path_title(_id int, _lang_id int, _skip_root boolean)
RETURNS varchar AS $$
DECLARE
  _title varchar;
BEGIN
  _title := '';

  while _id is not null and (_skip_root = false or exists(select * from folders where id = _id and parent_id is not null)) loop
    _title := (select name from vw_folders vw where vw.id = _id and vw.lang_id = _lang_id) || ' / ' || _title;
    _id := parent_id from folders where id = _id;
  end loop;

  _title := trim(both ' ' from  _title);
  RETURN trim(both '/' from  _title);
END;
$$ LANGUAGE plpgsql;

-- transfer_user_data
CREATE OR REPLACE FUNCTION transfer_user_data(_source_db varchar, _skip_doc_tables boolean) 
RETURNS void AS $$
  DECLARE _hostaddr varchar;
  DECLARE _port varchar;
  DECLARE _user varchar;
  DECLARE _password varchar;
  DECLARE _dblink_conn_str varchar;
BEGIN

  _hostaddr := '87.121.111.212';
  _port := '6432';
  _user := 'postgres';
  _password := 'L1m0n@d@';

  _dblink_conn_str := 'hostaddr='||_hostaddr||' port='||_port||' dbname='||_source_db||' user='||_user||' password='||_password;

  DELETE FROM opened_docs;
  DELETE FROM user_lang_preferences;
  DELETE FROM user_products;  
  DELETE FROM user_docs;
  DELETE FROM user_searches;
  DELETE FROM users;
  DELETE FROM client_products;
  DELETE FROM clients;
  DELETE FROM folder_langs;
  DELETE FROM folders;
  DELETE FROM products;

  -- products
  INSERT INTO products SELECT products.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM products') as products(product_id int, product_name varchar);

  -- clients
  INSERT INTO clients SELECT clients.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM clients') as clients(client_id int, client_name varchar);
  PERFORM setval('clients_client_id_seq', (SELECT MAX(client_id) FROM clients));

  -- client_products
  INSERT INTO client_products SELECT client_products.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM client_products') as client_products(id int, client_id int, product_id int, license_cnt int, start_date timestamp, end_date timestamp);
  PERFORM setval('client_products_id_seq', (SELECT MAX(id) FROM client_products));

  -- users
  INSERT INTO users SELECT users.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM users') as users(user_id int, client_id int, username varchar, password varchar, usertype_id int, fullname varchar, push_session boolean, max_login_count int, email varchar, session_timeout int);
  PERFORM setval('users_user_id_seq', (SELECT MAX(user_id) FROM users));

  -- user_searches
  INSERT INTO user_searches SELECT user_searches.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM user_searches') as user_searches(id int, user_id int, txt varchar, search_obj jsonb, search_date timestamp);
  PERFORM setval('user_searches_id_seq', (SELECT MAX(id) FROM user_searches));

  -- user_products
  INSERT INTO user_products SELECT user_products.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM user_products') as user_products(id int, user_id int, product_id int, license_cnt int);
  PERFORM setval('user_products_id_seq', (SELECT MAX(id) FROM user_products));

  -- user_lang_preferences
  DELETE FROM user_lang_preferences;
  INSERT INTO user_lang_preferences SELECT user_lang_preferences.* FROM dblink(_dblink_conn_str,
  'SELECT * FROM user_lang_preferences') as user_lang_preferences(id int, user_id int, lang int, ord int);
  PERFORM setval('user_lang_preferences_id_seq', (SELECT MAX(id) FROM user_lang_preferences));

  IF (_skip_doc_tables = false) THEN
    -- user_docs
    INSERT INTO user_docs SELECT user_docs.* FROM dblink(_dblink_conn_str,
    'SELECT * FROM user_docs') as user_docs(id int, user_id int, doc_lang_id int, add_date timestamp);
    PERFORM setval('user_docs_id_seq', (SELECT MAX(id) FROM user_docs));
  
    -- opened_docs
    INSERT INTO opened_docs SELECT opened_docs.* FROM dblink(_dblink_conn_str,
    'SELECT * FROM opened_docs') as opened_docs(id int, user_id int, doc_lang_id int, pinned boolean, open_date timestamp);
    PERFORM setval('opened_docs_id_seq', (SELECT MAX(id) FROM opened_docs));
  END IF;

  RETURN;
END;
$$ LANGUAGE plpgsql;


-- create_consumercases_folders
CREATE OR REPLACE FUNCTION create_consumercases_folders() 
RETURNS void AS $$
DECLARE _level_1_id int;
DECLARE _level_2_id int;
DECLARE _item varchar[];
DECLARE _arr varchar[];
DECLARE _i int;
BEGIN
  delete from folder_langs;
  delete from folders;

-- Consumer Legislation
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 1);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Consumer Legislation');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 2, 'Consumer Legislation');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 3, 'Consumer Legislation');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'Consumer Legislation');

  
  _arr := array[
  ['','European Union', ' +props:cEU', '1'],
  ['','Austria', ' +props:cAU', '2'],
  ['','Bulgaria', ' +props:cBG', '3'],
  ['','Germany', ' +props:cDE', '4'],
  ['','France', ' +props:cFR', '5'],
  ['','Italy', ' +props:cIT', '6'],
  ['','United Kingdom', ' +props:cUK', '7']
  ];

  FOREACH _item SLICE 1 IN ARRAY _arr
   LOOP
      insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, '+props:dtact'||_item[3], _item[4]::int);
      
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, _item[2]);
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 2, _item[2]);
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 3, _item[2]);
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, _item[2]);
   END LOOP;

-- Consumer Cases
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 1);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Consumer Cases');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 2, 'Consumer Cases');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 3, 'Consumer Cases');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'Consumer Cases');

  FOREACH _item SLICE 1 IN ARRAY _arr
   LOOP
      insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, '+props:dtjudgment'||_item[3], _item[4]::int);
      
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, _item[2]);
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 2, _item[2]);
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 3, _item[2]);
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, _item[2]);
   END LOOP;

-- Legal Doctrine
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 1);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Legal Doctrine');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 2, 'Legal Doctrine');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 3, 'Legal Doctrine');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'Legal Doctrine');

-- Articles
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, '+props:legdoc', 1);
      
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Articles');
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 2, 'Articles');
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 3, 'Articles');
      insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Articles');
  
END;
$$ LANGUAGE plpgsql;

-- create_folders
CREATE OR REPLACE FUNCTION create_folders() 
RETURNS void AS $$
DECLARE _level_1_id int;
DECLARE _level_2_id int;
DECLARE _item varchar[];
DECLARE _arr varchar[][][];
DECLARE _country_codes varchar[];
DECLARE _i int;
DECLARE _r record;
BEGIN
  delete from folder_langs;
  delete from folders;

  _country_codes := array['AT','BE','BG','UK','DE','IE','ES','IT','NL','PL','PT','FR','CZ'];

-- Законодателство на ЕС
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 1);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Законодателство на ЕС');
  --insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 2, 'EU-Recht');
  --insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 3, 'Droit de l''UE');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'EU law');

-- Основни актове
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 
  'classificators:(4ebeaa76db594b28a7f74a5afef50c4b)', 1);
  --'props:(dtact) props:(cEU) docnumber:(31995L0046 ||  32002L0058 || 32006L0024 || 12007P || 32008F0977 || 01995L0046 || 02002L0058 || 32009L0136 || 32011R0211 || 02011R0211 || 32013R0611 || 02009L0136 || 32016L0680 || 32016L0681 || 32016R0679 || 32016D1250 || 32016R0679R02 || 02016R0679 || 02016L0680)', 1);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Основни актове');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Key instruments');

-- Други
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 
  'classificators:(d8c615a1547943c580e0f42d8f6bc692)', 2);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Други');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Other instruments');

  -- Национално законодателство
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 2);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Национално законодателство');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'National law');
  
-- props:(dtjudgment) 

  -- sort by English name
  FOR _r IN select * from vw_countries where lang_id=4 and code = any(_country_codes) order by name 
  LOOP
    insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'props:(c'||_r.code||') props:(dtact)', null);
    --_level_2_id := currval('folders_id_seq');
    insert into folder_langs(folder_id, lang_id,  name)
    select currval('folders_id_seq'), lang_id, name from vw_countries where id = _r.id and lang_id in (1,2,3,4);
  END LOOP;

  -- Европейски комитет по защита на данните
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 3);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Европейски комитет по защита на данните');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'European Data Protection Board');

    -- Общи насоки
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'classificators:(ba8c4a71da404d27bdb151480f09a7cf)', 1);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Общи насоки');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'General Guidance');

    -- Обществени консултации
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'classificators:(dc37bc704b6c445180b6f554ee610899)', 2);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Обществени консултации');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Public Consultations');

    -- Работна група по член 29
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, null, null, 3);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Работна група по член 29');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Article 29 Working Party');

      -- Насоки
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_2_id, 1, 1, 'classificators:(fbda0efcb63f4d7092c0eee6fdd8a7ae)', 1);
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Насоки');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Guidelines');

      -- Други
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_2_id, 1, 1, 'classificators:(a16f1869bd364cbf99cb03ae35c98e00)', 2);
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Други');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Other documents');

  -- Национални надзорни органи
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 4);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Национални надзорни органи');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'National supervisory authorities');

  -- sort by English name
  FOR _r IN select * from vw_countries where lang_id=4 and code = any(_country_codes) order by name 
  LOOP
    insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'props:(c'||_r.code||') classificators:(0301d788b37248ebbe21f89e2141b1fe)', null);
    _level_2_id := currval('folders_id_seq');
    insert into folder_langs(folder_id, lang_id,  name)
    select _level_2_id, lang_id, name from vw_countries where id = _r.id and lang_id in (1,2,3,4);

    -- Становища и указания
    --insert into folders(parent_id, product_id, query_type, query, ord) values(_level_2_id, 1, 1, 'TODO', 1);
    --insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Становища и указания');
    --insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Guidelines and opinions');

    -- Практика
    --insert into folders(parent_id, product_id, query_type, query, ord) values(_level_2_id, 1, 1, 'props:(c'||_r.code||') props:(dtjudgment)', 2);
    --insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Практика');
    --insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Practice');
  END LOOP;

  -- Съдебна практика на ЕС
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 5);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Съдебна практика на ЕС');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'EU case law');

  -- Съд
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'classificators:(0c6cc9321ee34f9ba1fdb35b68f61578)', 1);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Съд');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Court of Justice');

  -- Общ съд
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'classificators:(36e413433d014bf0904b2e0a56983c41)', 2);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Общ съд');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'General Court');

  -- Съд на публичната служба
  insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'classificators:(00bbdbeb63b44069b1d9477228464929)', 3);
  _level_2_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 1, 'Съд на публичната служба');
  insert into folder_langs(folder_id, lang_id,  name) values(currval('folders_id_seq'), 4, 'Civil Service Tribunal');

  -- Практика на националните съдилища
  insert into folders(parent_id, product_id, query_type, query, ord) values(null, 1, null, null, 6);
  _level_1_id := currval('folders_id_seq');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 1, 'Практика на националните съдилища');
  insert into folder_langs(folder_id, lang_id, name) values(currval('folders_id_seq'), 4, 'National case law');

  -- sort by English name
  FOR _r IN select * from vw_countries where lang_id=4 and code = any(_country_codes) order by name 
  LOOP
    insert into folders(parent_id, product_id, query_type, query, ord) values(_level_1_id, 1, 1, 'props:(c'||_r.code||') props:(dtjudgment)', null);
    --_level_2_id := currval('folders_id_seq');
    insert into folder_langs(folder_id, lang_id,  name)
    select currval('folders_id_seq'), lang_id, name from vw_countries where id = _r.id and lang_id in (1,2,3,4);
  END LOOP;
  
  --raise notice '_level_1_id=%', _level_1_id;
  --raise notice '_level_2_id=%', _level_2_id;


END;
$$ LANGUAGE plpgsql;

-- get_classifiers_map
CREATE OR REPLACE FUNCTION get_classifiers_map()
RETURNS TABLE(classifier_id uuid, mapping_classifier_ids varchar[]) AS $$
BEGIN
  RETURN QUERY
  SELECT cm.classifier_id, array_agg(cm.mapping_classifier_id::varchar) FROM classifiers_map cm
  GROUP BY cm.classifier_id;
END;
$$ LANGUAGE plpgsql;

-- CREATE OR REPLACE function pref_lang_iter(_best_lang int[], _doc_lang_id int, _ord int)
-- returns int[]
-- language sql
-- as $f$
-- SELECT CASE WHEN _ord < _best_lang[1] THEN array[ _ord, _doc_lang_id ]
--             ELSE _best_lang
--        END;
-- $f$;
-- 
-- CREATE OR REPLACE FUNCTION pref_lang_final(_best_lang int[])
-- returns int
-- language sql
-- as $f$
-- SELECT _best_lang[2];
-- $f$;
-- 
-- DROP AGGREGATE pref_doc_lang_id(int, int);
-- CREATE AGGREGATE pref_doc_lang_id (int, int)
-- (
--     sfunc = pref_lang_iter,
--     stype = int[],
--     finalfunc = pref_lang_final,
--     initcond = '{100,0}'
-- );


CREATE OR REPLACE FUNCTION get_doc_text(
    _doc_lang_id integer,
    _plain_xml boolean,
    _product_id integer,
    _show_free_documents boolean)
  RETURNS text AS $$
DECLARE
  _doc_id       int;
  _base_act_id  int;
  _doc_type int;
  _meta_tags text[];
  _start_tags text;
  _end_tags text;
  _art_pattern varchar;
  _art_pattern_alternative varchar;
  _doc_number varchar;
  _base_act_doc_number varchar;
BEGIN
  SELECT doc_number INTO _doc_number FROM vw_doc_langs WHERE doc_lang_id = _doc_lang_id;
  _art_pattern := '(?:^|_)art_([^_]+)';
  _art_pattern_alternative := '(?:^|_)art([^_]+)';
  IF EXISTS(SELECT * FROM cons_versions WHERE lead_doc_lang_id <> doc_lang_id AND doc_lang_id = _doc_lang_id) THEN
    raise notice 'start cons: %', timeofday();
    --
    SELECT lead_doc_lang_id INTO _base_act_id FROM cons_versions WHERE doc_lang_id = _doc_lang_id;
    SELECT celex INTO _base_act_doc_number FROM cons_versions WHERE doc_lang_id = _base_act_id;
    IF _base_act_doc_number IS NULL THEN _base_act_doc_number := _doc_number; END IF;
    SELECT vw.doc_id, vw.doc_type INTO _doc_id, _doc_type FROM vw_doc_langs_mat vw WHERE vw.doc_lang_id = _base_act_id;

    _meta_tags := get_doc_meta_tags(_doc_type);
    _start_tags := _meta_tags[1];
    _end_tags := _meta_tags[2];
    
    CREATE TEMP TABLE _t_base_pars(doc_par_id int, base_doc_par_id int, base_doc_to_par_art character varying);
    
    INSERT INTO _t_base_pars
    SELECT DISTINCT dp.id, pt2.doc_par_id, pt2.art_eid
    --,get_pid_from_art_number(_base_act_id, pt.eid) 
    FROM doc_pars dp JOIN par_texts pt ON dp.par_text_id=pt.id 
                LEFT JOIN (SELECT substring(pt.eid from _art_pattern) art_eid, dp.id as doc_par_id 
                           FROM doc_pars dp JOIN par_texts pt ON dp.par_text_id=pt.id AND dp.doc_lang_id = _base_act_id 
                           WHERE substring(pt.eid from _art_pattern) IS NOT NULL) pt2 ON pt2.art_eid = substring(pt.eid from _art_pattern)
    WHERE dp.doc_lang_id=_doc_lang_id;
    
    CREATE INDEX ON _t_base_pars(doc_par_id);

    raise notice '_t_base_pars done time: %', timeofday();
    
    -- return base act links
    return array_to_string(_start_tags || array(
    SELECT CASE WHEN p.subtype = 5 AND (EXISTS(SELECT * FROM vw_doc_links_mat vw JOIN product_docs pd ON pd.doc_lang_id = vw.doc_lang_id AND pd.product_id = _product_id
                                                              JOIN vw_doc_langs_mat dl ON vw.doc_lang_id=dl.doc_lang_id
                                               WHERE vw.to_doc_lang_id = _base_act_id AND dl.doc_id <> _doc_id AND vw.to_doc_par_id = t.base_doc_par_id
                           AND (_show_free_documents = TRUE OR EXISTS(SELECT * FROM doc_classifiers dc where dc.doc_lang_id = vw.doc_lang_id AND dc.classifier_id = '28374142-2597-4C8B-B4D3-38D7F729A7D9') = FALSE)
                                               )  
                                         --OR EXISTS(SELECT * FROM get_law_relations_docs_recursive(_doc_number, lower(cyr2lat(t.base_doc_to_par_art))) vw
                                        -- WHERE _show_free_documents = TRUE OR EXISTS (
                                        -- SELECT * FROM doc_classifiers WHERE doc_lang_id = vw.doc_lang_id AND classifier_id = '28374142-2597-4C8B-B4D3-38D7F729A7D9') = FALSE
                                         --) -- WRITE IT 1
                                         
                                        -- OR EXISTS(SELECT * FROM get_law_relations_docs_recursive(dl.doc_number, lower(cyr2lat(substring(vw.to_par from _art_pattern)))))
                                        --OR EXISTS(SELECT * FROM vw_law_relations vw WHERE lower(vw.to_celex)=lower(_doc_number) AND lower(to_article) = lower(cyr2lat(substring(t.eid from _art_pattern))))
                                        )
           THEN
             regexp_replace(pt.content, '^(\s*?<\w+?\s)', '\1' || 'in_link="' || t.base_doc_par_id::varchar || '" in_link_doc_lang_id="' || _base_act_id::varchar || '" pid="' || pt.id::varchar || '" ') 
           ELSE 
             regexp_replace(pt.content, '^(\s*?<\w+?\s)', '\1' || 'pid="' || pt.id::varchar || '" ') 
           END  
    FROM doc_pars p JOIN par_texts pt ON p.par_text_id = pt.id 
               LEFT JOIN _t_base_pars t ON t.doc_par_id = p.id
    WHERE p.doc_lang_id = _doc_lang_id ORDER BY p.ord
    ) || _end_tags, '', '');

    raise notice 'result done: %', timeofday();

    DROP TABLE _t_base_pars;
  ELSE
    --
    raise notice 'start: %', timeofday();
    
    SELECT vw.doc_id, vw.doc_type INTO _doc_id, _doc_type FROM vw_doc_langs_mat vw WHERE vw.doc_lang_id = _doc_lang_id;

    _meta_tags := get_doc_meta_tags(_doc_type);
    _start_tags := _meta_tags[1];
    _end_tags := _meta_tags[2];
    
    return array_to_string(_start_tags || array(
    SELECT CASE WHEN p.subtype = 5 AND SUBSTRING(t.eid FROM 'art_([^_]+)') IS NOT NULL AND 
                     (EXISTS(SELECT * FROM vw_doc_links_mat vw JOIN product_docs pd ON pd.doc_lang_id = vw.doc_lang_id AND pd.product_id = _product_id
                                                              JOIN vw_doc_langs_mat dl ON vw.doc_lang_id=dl.doc_lang_id
                            WHERE vw.to_doc_lang_id = _doc_lang_id AND dl.doc_id <> _doc_id AND vw.to_doc_par_id = p.id
                            AND (_show_free_documents = TRUE OR EXISTS(SELECT * FROM doc_classifiers dc where dc.doc_lang_id = vw.doc_lang_id AND dc.classifier_id = '28374142-2597-4C8B-B4D3-38D7F729A7D9') = FALSE))  
                            --OR EXISTS(SELECT * FROM get_law_relations_docs_recursive(_doc_number, lower(cyr2lat(substring(t.eid from _art_pattern)))) vw)
                             --   WHERE _show_free_documents = TRUE OR EXISTS (
                                        -- SELECT * FROM doc_classifiers WHERE doc_lang_id = vw.doc_lang_id AND classifier_id = '28374142-2597-4C8B-B4D3-38D7F729A7D9') = FALSE) -- WRITE IT 2
                             
                            )
           THEN regexp_replace(t.content, '^(\s*?<\w+?\s)', '\1' || 'in_link="' || p.id::varchar || '" pid="' || p.id::varchar || '" ') 
           ELSE regexp_replace(t.content, '^(\s*?<\w+?\s)', '\1' || 'pid="' || p.id::varchar || '" ') END  
    FROM doc_pars p JOIN par_texts t ON p.par_text_id = t.id WHERE p.doc_lang_id = _doc_lang_id ORDER BY p.ord
    ) || _end_tags, '', '');

    raise notice 'result done: %', timeofday();

  END IF;


--SELECT CASE WHEN EXISTS(SELECT * FROM vw_doc_links_mat vw WHERE vw.to_doc_lang_id = _doc_lang_id AND vw.to_doc_par_id = p.id) 
--       THEN regexp_replace(t.content, '^(\s*?<\w+?\s)', '\1' || 'in_link="' || p.id::varchar || '" pid="' || p.id::varchar || '" ') 
--       ELSE regexp_replace(t.content, '^(\s*?<\w+?\s)', '\1' || 'pid="' || p.id::varchar || '" ') END  
--FROM doc_pars p JOIN par_texts t ON p.par_text_id = t.id WHERE p.doc_lang_id = _doc_lang_id ORDER BY p.ord
--), '', '');

END;
$$ LANGUAGE plpgsql VOLATILE;