INCLUDE ffl/tst.fs
REQUIRE set.fs

." --- set --- " CR

T{ ." bit-value" CR
  0  BIT-VALUE     1 ?U
  1  BIT-VALUE     2 ?U
  16 BIT-VALUE 65536 ?U
}T

T{ ." in-set?" CR
   0 1 IN-SET? ?FALSE
 255 1 IN-SET? ?TRUE 
 128 7 IN-SET? ?TRUE
 128 6 IN-SET? ?FALSE
}T

T{ ." in-set!" CR
    1 0 IN-SET! 1 IN-SET? ?TRUE
    4 0 IN-SET!  7 SWAP IN-SET! 
    DUP 4 IN-SET? ?TRUE
    DUP 7 IN-SET? ?TRUE
        8 IN-SET? ?FALSE
}T

T{ ." full-set" CR
  4 FULL-SET 
  DUP 0 IN-SET? ?TRUE
  DUP 1 IN-SET? ?TRUE
  DUP 2 IN-SET? ?TRUE
  DUP 3 IN-SET? ?TRUE
      4 IN-SET? ?FALSE
}T

T{ ." empty-set" CR
  EMPTY-SET 0 IN-SET? ?FALSE
  EMPTY-SET 4 IN-SET? ?FALSE
}T

T{ ." union-set" CR
  17 EMPTY-SET IN-SET!
  23 SWAP IN-SET!
  42 EMPTY-SET IN-SET!
   5 SWAP IN-SET!
  UNION-SET
  DUP  5 IN-SET? ?TRUE
  DUP 17 IN-SET? ?TRUE
  DUP 23 IN-SET? ?TRUE
  DUP 42 IN-SET? ?TRUE
      16 IN-SET? ?FALSE
}T

T{ ." set-items" CR
  17 EMPTY-SET IN-SET!
  23 SWAP IN-SET!
  42 EMPTY-SET IN-SET!
   5 SWAP IN-SET!
  UNION-SET
  64 SET-ITEMS
  42 ?S 23 ?S 17 ?S 5 ?S
}T
