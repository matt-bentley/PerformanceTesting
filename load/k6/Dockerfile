FROM loadimpact/k6:latest
COPY load/scripts /scripts
WORKDIR /scripts
ENTRYPOINT ["k6"]
CMD ["run", "test.js"]