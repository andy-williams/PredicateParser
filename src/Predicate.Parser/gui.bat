antlr4 -Dlanguage=Java -o .java Predicate.g4 -no-listener -visitor ^
& javac .java/*.java ^
& pushd .java ^
& grun Predicate expr -gui