DROP TABLE IF EXISTS public.vdp03;
CREATE TABLE public.vdp03
(
    id integer NOT NULL,
    dateandtime timestamp without time zone,
    mks integer,
    tagname character varying(50) COLLATE pg_catalog."default",
    val double precision,
    CONSTRAINT vdp03_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.vdp03
    OWNER to postgres;
CREATE INDEX time03
    ON public.vdp03 USING btree
    (dateandtime)
    TABLESPACE pg_default;
\copy vdp03 from 'vdp03copy.csv' DELIMITER ',' CSV;	