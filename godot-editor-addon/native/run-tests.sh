#!/bin/bash

GODOT=${GODOT:-godot}

END_STRING="==== rider-addon-tests finished ===="
FAILURE_STRING="******** FAILED ********"

cd "$(dirname "$0")/.."

OUTPUT=$($GODOT --editor --headless --rider-addon-tests --quit)
ERRCODE=$?

echo "$OUTPUT"
echo

if ! echo "$OUTPUT" | grep -e "$END_STRING" >/dev/null; then
    echo "ERROR: Tests failed to complete"
    exit 1
fi

if echo "$OUTPUT" | grep -e "$FAILURE_STRING" >/dev/null; then
    exit 1
fi

# Success!
exit 0
