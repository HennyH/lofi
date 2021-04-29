# there are build issues around compiling the lame and ssl dependencies on alpine
FROM ocaml/opam:ubuntu as builder
ARG PACKAGES="taglib mad lame vorbis cry samplerate opus fdkaac faad flac liquidsoap yojson ssl ffmpeg"
RUN sudo apt-get update \
    && opam depext --install $PACKAGES

RUN ROOT=/home/opam/root \
    && mkdir -p $ROOT/liquidsoap/bin \
    && eval $(opam env) \
    && mv "$(command -v liquidsoap)" $ROOT/liquidsoap/bin \
    && opam depext --list $PACKAGES > $ROOT/liquidsoap/depexts \
    && mkdir -p $ROOT/$OPAM_SWITCH_PREFIX/lib \
    && mv $OPAM_SWITCH_PREFIX/share $ROOT/$OPAM_SWITCH_PREFIX \
    && mv $OPAM_SWITCH_PREFIX/lib/liquidsoap $ROOT/$OPAM_SWITCH_PREFIX/lib

FROM ubuntu:hirsute
COPY --from=builder /home/opam/root /
RUN apt-get update \
    && cat /liquidsoap/depexts | xargs apt-get install -y --no-install-recommends \
    && ln -s /liquidsoap/bin/liquidsoap /usr/local/bin/liquidsoap
RUN apt-get install -y --no-install-recommends curl
COPY stream.liq playlist.pls /etc/liquidsoap/
ENTRYPOINT [ "liquidsoap" ]
CMD [ "/etc/liquidsoap/stream.liq" ]