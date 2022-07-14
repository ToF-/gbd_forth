INCLUDE ffl/tst.fs
REQUIRE numbers.fs

." --- numbers ---" CR

T{ ." s>number?" CR
  S" 42"  S>NUMBER? ?TRUE 42 S>D ?D
  S"  17" SKIP-SPACES S>NUMBER? ?TRUE 17 S>D ?D
  S" -23" S>NUMBER? ?TRUE -23 S>D ?D
  S" 42f" S>NUMBER? ?FALSE 42 S>D ?D
}T

T{ ." skip-spaces" CR
  S"   Foo  Bar " 2DUP SKIP-SPACES ROT SWAP - 2 ?S SWAP - 2 ?S
  S" " 2DUP SKIP-SPACES SWAP -ROT = -ROT = AND ?TRUE
  S"       " SKIP-SPACES NIP 0 ?S
}T

T{ ." skip-non-spaces" CR
 S" Foo Bar" 2DUP SKIP-NON-SPACES ROT SWAP - 3 ?S SWAP - 3 ?S
 S"     Bar" 2DUP SKIP-NON-SPACES SWAP -ROT = -ROT = AND ?TRUE
 S" "        2DUP SKIP-NON-SPACES SWAP -ROT = -ROT = AND ?TRUE
}T

T{ ." s>next-number?" CR
  S" 42" 2DUP S>NEXT-NUMBER? ?TRUE 42 S>D ?D ROT SWAP - 2 ?S SWAP - 2 ?S
  S"    42  " S>NEXT-NUMBER? ?TRUE 42 S>D ?D 2DROP
  S"   " 2DUP S>NEXT-NUMBER? ?FALSE 0 S>D ?D ROT SWAP - 2 ?S SWAP - 2 ?S
  S"  FOO" 2DUP S>NEXT-NUMBER? ?FALSE 0 S>D ?D ROT SWAP - 4 ?S SWAP - 4 ?S
}T

T{ ." s>numbers!" CR
  CREATE ARRAY 3 2* CELLS ALLOT
  S"   42   23   -17  " ARRAY S>NUMBERS! 3 ?S
  ARRAY 0 2* CELLS + 2@ 42 S>D ?D
  ARRAY 1 2* CELLS + 2@ 23 S>D ?D
  ARRAY 2 2* CELLS + 2@ -17 S>D ?D
}T


