grammar Predicate;

/* 
PARSER RULES
 */

expr: predicate EOF;

predicate
    : predicate booleanOperator predicate
    | OpenParen predicate CloseParen
    | operand operator operand
    ;

booleanOperator
    : And
    | Or
    ;

operator
    : GreaterThan
    | LessThan
    | GreaterThanEqual
    | LessThanEqual
    | Equal
    | NotEqual
    | Contains
    ;

operand
    : Property
    | String
    | Number
    ;

/*
LEXER RULES
 */
 
 // boolean operators
And: 'and';
Or: 'or';

// operators 
GreaterThan: '>';
GreaterThanEqual: '>=';
LessThan: '<'; 
LessThanEqual: '<=';
Equal: '=';
NotEqual: '!=';
Contains: 'contains';

OpenParen: '(';
CloseParen: ')'; 

// operands
Number: [0-9]+;
String: '"' ( '\\"' | ~["\r\n] )*? '"';
Property: AT PROPERTY_NAME;

WhiteSpace: [ \t\f\r\n]+ -> channel(HIDDEN); // skip whitespaces
Discardable: . -> channel(HIDDEN); // keeping whitespace tokenised makes it easier for syntax highlighting
    
fragment PROPERTY_NAME: [a-z_]+;
fragment AT: '@';
