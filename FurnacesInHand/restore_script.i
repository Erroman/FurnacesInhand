DROP TABLE IF EXISTS public.vdp00;
DROP SEQUENCE IF EXISTS public.vdp00_id_seq;
CREATE SEQUENCE public.vdp00_id_seq;
ALTER SEQUENCE public.vdp00_id_seq
    OWNER TO postgres;
CREATE TABLE public.vdp00
(
    id integer NOT NULL DEFAULT nextval('vdp00_id_seq'::regclass),
    dateandtime timestamp without time zone,
    mks integer,
    tagname character varying(50) COLLATE pg_catalog."default",
    val double precision,
    CONSTRAINT "PK_public.vdp00" PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;
\copy vdp00 from 'vdp00copy.csv' DELIMITER ',' CSV;	
ALTER TABLE public.vdp00
    OWNER to postgres;
CREATE INDEX time00
    ON public.vdp00 USING btree
    (dateandtime)
    TABLESPACE pg_default;
