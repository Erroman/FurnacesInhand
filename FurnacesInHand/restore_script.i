DROP TABLE IF EXISTS public.vdp00;
CREATE TABLE public.vdp00
(
    id integer NOT NULL,
    dateandtime timestamp without time zone,
    mks integer,
    tagname character varying(50) COLLATE pg_catalog."default",
    val double precision,
    CONSTRAINT vdp00_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.vdp00
    OWNER to postgres;
CREATE INDEX time00
    ON public.vdp00 USING btree
    (dateandtime)
    TABLESPACE pg_default;
\copy vdp00 from 'vdp00copy.csv' DELIMITER ',' CSV;	