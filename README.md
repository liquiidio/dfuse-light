# dfuse-light
High-performance streaming- and history-solution for EOSIO-based chains leveraging deep-mind-loggging while reducing the overhead of hosting dfuse or firehose

## Description
dfuse-light reads and deserializes deep-mind logs (actions, traces, deltas, permission-operations, limit-ops etc.) and re-constructs eosio-blocks similar to dfuse.

dfuse-light postprocesses and "flattens" blocks and transactions to serialize and store the relevant data while reducing storage size and increasing performance.

Postprocessed data is serialized and highly compressed to reduce the storage-consumption.
