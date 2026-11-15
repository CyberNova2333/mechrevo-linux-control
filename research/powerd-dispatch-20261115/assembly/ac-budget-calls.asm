
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004de1d9 <.text+0xd8369>:
  4de1d9:	41 89 43 58          	mov    %eax,0x58(%r11)
  4de1dd:	41 8b 43 28          	mov    0x28(%r11),%eax
  4de1e1:	89 fa                	mov    %edi,%edx
  4de1e3:	41 8b 76 3c          	mov    0x3c(%r14),%esi
  4de1e7:	44 89 54 24 0c       	mov    %r10d,0xc(%rsp)
  4de1ec:	41 8b 8b 7c 02 00 00 	mov    0x27c(%r11),%ecx
  4de1f3:	45 8b 83 34 03 00 00 	mov    0x334(%r11),%r8d
  4de1fa:	50                   	push   %rax
  4de1fb:	41 8b 43 24          	mov    0x24(%r11),%eax
  4de1ff:	50                   	push   %rax
  4de200:	e8 5b f6 ff ff       	call   4dd860 <strtol@plt+0xd79f8>
  4de205:	5a                   	pop    %rdx
  4de206:	59                   	pop    %rcx
  4de207:	8d b8 fa 00 00 00    	lea    0xfa(%rax),%edi
  4de20d:	44 8b 54 24 0c       	mov    0xc(%rsp),%r10d
  4de212:	41 8b b3 a4 00 00 00 	mov    0xa4(%r11),%esi
  4de219:	48 69 ff d3 4d 62 10 	imul   $0x10624dd3,%rdi,%rdi
  4de220:	89 74 24 04          	mov    %esi,0x4(%rsp)
  4de224:	48 c1 ef 25          	shr    $0x25,%rdi
  4de228:	69 ff f4 01 00 00    	imul   $0x1f4,%edi,%edi
  4de22e:	44 39 d7             	cmp    %r10d,%edi
  4de231:	44 0f 46 d7          	cmovbe %edi,%r10d
  4de235:	41 39 f2             	cmp    %esi,%r10d
  4de238:	44 89 d7             	mov    %r10d,%edi
  4de23b:	0f 42 fe             	cmovb  %esi,%edi
  4de23e:	8b 34 24             	mov    (%rsp),%esi
  4de241:	41 89 7b 54          	mov    %edi,0x54(%r11)
  4de245:	85 f6                	test   %esi,%esi
  4de247:	0f 84 7b 08 00 00    	je     4deac8 <strtol@plt+0xd8c60>
  4de24d:	41 8b 83 2c 01 00 00 	mov    0x12c(%r11),%eax
  4de254:	85 c0                	test   %eax,%eax
  4de256:	0f 84 8c 07 00 00    	je     4de9e8 <strtol@plt+0xd8b80>
  4de25c:	83 c0 3e             	add    $0x3e,%eax
  4de25f:	41 8b 93 a8 00 00 00 	mov    0xa8(%r11),%edx
  4de266:	8b 74 24 04          	mov    0x4(%rsp),%esi
  4de26a:	48 69 c0 d3 4d 62 10 	imul   $0x10624dd3,%rax,%rax
  4de271:	48 c1 e8 23          	shr    $0x23,%rax
  4de275:	6b c0 7d             	imul   $0x7d,%eax,%eax
  4de278:	39 d0                	cmp    %edx,%eax
  4de27a:	0f 47 c2             	cmova  %edx,%eax
  4de27d:	39 f0                	cmp    %esi,%eax
  4de27f:	0f 42 c6             	cmovb  %esi,%eax
  4de282:	89 44 24 0c          	mov    %eax,0xc(%rsp)
  4de286:	41 89 43 74          	mov    %eax,0x74(%r11)
  4de28a:	41 8b 43 40          	mov    0x40(%r11),%eax
  4de28e:	41 8b 76 44          	mov    0x44(%r14),%esi
  4de292:	41 89 d9             	mov    %ebx,%r9d
  4de295:	41 8b 8b 78 02 00 00 	mov    0x278(%r11),%ecx
  4de29c:	41 8b 93 c0 01 00 00 	mov    0x1c0(%r11),%edx
  4de2a3:	50                   	push   %rax
  4de2a4:	41 8b 43 3c          	mov    0x3c(%r11),%eax
  4de2a8:	45 8b 83 30 03 00 00 	mov    0x330(%r11),%r8d
  4de2af:	50                   	push   %rax
  4de2b0:	e8 ab f5 ff ff       	call   4dd860 <strtol@plt+0xd79f8>
  4de2b5:	5a                   	pop    %rdx
  4de2b6:	59                   	pop    %rcx
  4de2b7:	41 89 e9             	mov    %ebp,%r9d
  4de2ba:	8d 98 fa 00 00 00    	lea    0xfa(%rax),%ebx
  4de2c0:	41 8b 43 30          	mov    0x30(%r11),%eax
  4de2c4:	41 8b 8b 84 02 00 00 	mov    0x284(%r11),%ecx
  4de2cb:	48 69 db d3 4d 62 10 	imul   $0x10624dd3,%rbx,%rbx
  4de2d2:	41 8b 93 cc 01 00 00 	mov    0x1cc(%r11),%edx
  4de2d9:	45 8b 83 3c 03 00 00 	mov    0x33c(%r11),%r8d
  4de2e0:	48 c1 eb 25          	shr    $0x25,%rbx
  4de2e4:	69 db f4 01 00 00    	imul   $0x1f4,%ebx,%ebx
  4de2ea:	41 89 5b 50          	mov    %ebx,0x50(%r11)
  4de2ee:	41 8b 76 4c          	mov    0x4c(%r14),%esi
  4de2f2:	50                   	push   %rax
  4de2f3:	41 8b 43 2c          	mov    0x2c(%r11),%eax
  4de2f7:	50                   	push   %rax
  4de2f8:	e8 63 f5 ff ff       	call   4dd860 <strtol@plt+0xd79f8>
  4de2fd:	5e                   	pop    %rsi
  4de2fe:	41 58                	pop    %r8
  4de300:	45 89 f9             	mov    %r15d,%r9d
  4de303:	8d a8 fa 00 00 00    	lea    0xfa(%rax),%ebp
  4de309:	41 8b 43 38          	mov    0x38(%r11),%eax
  4de30d:	41 8b 8b 88 02 00 00 	mov    0x288(%r11),%ecx
  4de314:	48 69 ed d3 4d 62 10 	imul   $0x10624dd3,%rbp,%rbp
  4de31b:	41 8b 93 d0 01 00 00 	mov    0x1d0(%r11),%edx
  4de322:	45 8b 83 40 03 00 00 	mov    0x340(%r11),%r8d
  4de329:	48 c1 ed 25          	shr    $0x25,%rbp
  4de32d:	69 ed f4 01 00 00    	imul   $0x1f4,%ebp,%ebp
  4de333:	41 89 6b 5c          	mov    %ebp,0x5c(%r11)
  4de337:	41 8b 76 48          	mov    0x48(%r14),%esi
  4de33b:	50                   	push   %rax
  4de33c:	41 8b 43 34          	mov    0x34(%r11),%eax
  4de340:	50                   	push   %rax
  4de341:	e8 1a f5 ff ff       	call   4dd860 <strtol@plt+0xd79f8>
  4de346:	41 59                	pop    %r9
  4de348:	41 5a                	pop    %r10
  4de34a:	8b 74 24 04          	mov    0x4(%rsp),%esi
  4de34e:	39 f5                	cmp    %esi,%ebp
  4de350:	0f 42 ee             	cmovb  %esi,%ebp
  4de353:	44 39 ed             	cmp    %r13d,%ebp
  4de356:	41 0f 47 ed          	cmova  %r13d,%ebp
  4de35a:	05 fa 00 00 00       	add    $0xfa,%eax
  4de35f:	48 69 c0 d3 4d 62 10 	imul   $0x10624dd3,%rax,%rax
  4de366:	41 89 6b 5c          	mov    %ebp,0x5c(%r11)
  4de36a:	48 c1 e8 25          	shr    $0x25,%rax
  4de36e:	69 c0 f4 01 00 00    	imul   $0x1f4,%eax,%eax
  4de374:	44 39 e8             	cmp    %r13d,%eax
  4de377:	44 0f 46 e8          	cmovbe %eax,%r13d
  4de37b:	41 39 ed             	cmp    %ebp,%r13d
  4de37e:	44 0f 42 ed          	cmovb  %ebp,%r13d
  4de382:	80 7c 24 0b 00       	cmpb   $0x0,0xb(%rsp)
  4de387:	0f 84 13 06 00 00    	je     4de9a0 <strtol@plt+0xd8b38>
  4de38d:	39 fb                	cmp    %edi,%ebx
  4de38f:	0f 42 df             	cmovb  %edi,%ebx
  4de392:	41 39 dd             	cmp    %ebx,%r13d
  4de395:	41 89 5b 50          	mov    %ebx,0x50(%r11)
  4de399:	44 0f 47 eb          	cmova  %ebx,%r13d
  4de39d:	45 89 6b 60          	mov    %r13d,0x60(%r11)
