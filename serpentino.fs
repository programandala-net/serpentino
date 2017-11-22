#! /usr/bin/env gforth

\ Serpentino

\ Description: A simple snake game written in Forth for Gforth
\ and Solo Forth. Under development.

\ Author: Marcos Cruz (programandala.net)
\ http://programandala.net
\ http://github.com/programandala-net/serpentino 

\ Last modified 201711221420

\ =============================================================
\ License

\ You may do whatever you want with this work, so long as you
\ retain all copyright, credit and authorship notices, and this
\ license.  There is no warranty.

\ =============================================================
\ Credit

\ Original code by Robert Pfeiffer:
\ <https://github.com/robertpfeiffer/forthsnake>.

\ ==============================================================

: random-range ( n1 n2 -- n3 ) over - utime + swap mod + ;

variable delay
  \ Frame delay in ms.

200 constant initial-delay
  \ Intial value of `delay`, in ms.

200 constant max-length

cols 2 - constant arena-width

rows 4 - constant arena-height

2 cells constant /segment

create snake  max-length /segment * allot
  \ Snake's segments. Each segment contains its coordinates.

2variable apple
  \ Coordinates of the apple.

variable head
  \ Head segment.

variable length
  \ Snake's current length.

variable direction
  \ Snake's current direction, as an execution token that
  \ returns the coordinates' increments.

: segment ( n -- a )
  head @ + max-length mod /segment * snake + ;

: clash? ( a1 a2 -- f ) 2@ rot 2@ d= ;
  \ Are the coordinates contained in _a1_ equal to the
  \ coordinates contained in _a2_?

: head* ( -- a ) 0 segment ;
  \ _a_ is the address of the head segment.

: cross? ( a -- f ) head* clash? ;
  \ Does the head cross segment _a_?

: random-coordinates ( -- x y ) 1 arena-width  random-range
                                1 arena-height random-range ;

: new-apple ( -- ) random-coordinates apple 2! ;

: grow ( -- ) 1 length +! ;

: eat-apple ( -- ) grow new-apple ;

: coords+ ( n1 n2 x1 y1 -- x2 y2 ) rot + -rot + swap ;
  \ Update coordinates _x1 y1_ with increments _n1 n2_,
  \ resulting coordinates _x2 y2_.

: move-head ( -- ) head @ 1- max-length mod head ! ;

: step ( n1 n2 -- ) head* 2@ move-head coords+ head* 2! ;

-1  0 2constant left

 1  0 2constant right

 0  1 2constant down

 0 -1 2constant up

: wall? ( -- f ) head* 2@ 1 arena-height within swap
                          1 arena-width  within and 0= ;

: crossing? ( -- f )
  length @ 1 ?do  i segment cross? if unloop true exit then
             loop false ;

: apple? ( -- f ) head* apple clash? ;

: dead? ( -- f ) wall? crossing? or ;

: .frame ( -- )
  0 0 at-xy
  arena-width  0 ?do ." +" loop
  arena-height 0 ?do arena-width i at-xy ." +" cr ." +" loop
  arena-width  0 ?do ." +" loop cr ;

: .snake ( -- )
  0 segment 2@ at-xy ." O"
  length @ 1 ?do i segment 2@ at-xy ." o" loop ;

: .apple ( -- ) apple 2@ at-xy ." Q" ;

: render ( -- )
  page .snake .apple .frame cr length @ . ;

: init ( -- )
  initial-delay delay !
  0 head !
  arena-width 2 / arena-height 2 / snake 2!
  3 3 apple 2!
  3 length !
  ['] up direction !
  left step left step left step left step ;

: (rudder) ( -- xt )
  key case
    '4' of ['] left  endof
    '6' of ['] up    endof
    '5' of ['] right endof
    '7' of ['] down  endof
    direction @ swap
  endcase ;

: rudder ( -- n1 n2 ) key? if   (rudder) direction !
                           then direction perform ;

: lazy ( -- ) delay @ ms ;

: (game) ( -- ) render lazy rudder step
                apple? if eat-apple then ;

: type-center ( ca len -- ) cols over - 2/ rows 2/ at-xy type ;

: game-over ( -- ) s" **** GAME OVER **** " type-center
                   2000 ms key drop ;

: game ( -- ) begin (game) dead? until game-over ;

: splash-screen ( -- )
  page s" oooO SERPENTINO Oooo" type-center space 2000 ms ;

: run ( -- ) begin splash-screen init game again ;

\ =============================================================
\ Change log

\ 2017-11-22: Fork from Robert Pfeiffer's forthsnake
\ (https://github.com/robertpfeiffer/forthsnake). Change source
\ style.  Rename words. Factor. Use constants and variables.
\ Use full screen. Draw the head apart.
