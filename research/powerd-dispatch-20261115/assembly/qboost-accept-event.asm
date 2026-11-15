
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004d9500 <.text+0xd3690>:
  4d9500:	55                   	push   %rbp
  4d9501:	53                   	push   %rbx
  4d9502:	48 89 fb             	mov    %rdi,%rbx
  4d9505:	48 83 ec 08          	sub    $0x8,%rsp
  4d9509:	8b 4c 24 44          	mov    0x44(%rsp),%ecx
  4d950d:	0f b6 54 24 4c       	movzbl 0x4c(%rsp),%edx
  4d9512:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d9515:	85 c9                	test   %ecx,%ecx
  4d9517:	8d 41 01             	lea    0x1(%rcx),%eax
  4d951a:	8b 4c 24 2c          	mov    0x2c(%rsp),%ecx
  4d951e:	0f 48 c6             	cmovs  %esi,%eax
  4d9521:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d9524:	85 c9                	test   %ecx,%ecx
  4d9526:	89 87 d0 02 00 00    	mov    %eax,0x2d0(%rdi)
  4d952c:	8d 41 01             	lea    0x1(%rcx),%eax
  4d952f:	8b 4c 24 34          	mov    0x34(%rsp),%ecx
  4d9533:	0f 48 c6             	cmovs  %esi,%eax
  4d9536:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d9539:	85 c9                	test   %ecx,%ecx
  4d953b:	89 87 d8 02 00 00    	mov    %eax,0x2d8(%rdi)
  4d9541:	8d 41 01             	lea    0x1(%rcx),%eax
  4d9544:	8b 4c 24 3c          	mov    0x3c(%rsp),%ecx
  4d9548:	0f 48 c6             	cmovs  %esi,%eax
  4d954b:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d954e:	85 c9                	test   %ecx,%ecx
  4d9550:	89 87 dc 02 00 00    	mov    %eax,0x2dc(%rdi)
  4d9556:	8d 41 01             	lea    0x1(%rcx),%eax
  4d9559:	8b 4c 24 48          	mov    0x48(%rsp),%ecx
  4d955d:	0f 48 c6             	cmovs  %esi,%eax
  4d9560:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d9563:	85 c9                	test   %ecx,%ecx
  4d9565:	89 87 e0 02 00 00    	mov    %eax,0x2e0(%rdi)
  4d956b:	8d 41 01             	lea    0x1(%rcx),%eax
  4d956e:	8b 4c 24 30          	mov    0x30(%rsp),%ecx
  4d9572:	0f 48 c6             	cmovs  %esi,%eax
  4d9575:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d9578:	85 c9                	test   %ecx,%ecx
  4d957a:	89 87 d4 02 00 00    	mov    %eax,0x2d4(%rdi)
  4d9580:	8d 41 01             	lea    0x1(%rcx),%eax
  4d9583:	8b 4c 24 38          	mov    0x38(%rsp),%ecx
  4d9587:	0f 48 c6             	cmovs  %esi,%eax
  4d958a:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d958d:	85 c9                	test   %ecx,%ecx
  4d958f:	89 87 e4 02 00 00    	mov    %eax,0x2e4(%rdi)
  4d9595:	8d 41 01             	lea    0x1(%rcx),%eax
  4d9598:	8b 4c 24 40          	mov    0x40(%rsp),%ecx
  4d959c:	0f 48 c6             	cmovs  %esi,%eax
  4d959f:	85 c9                	test   %ecx,%ecx
  4d95a1:	8d 71 ff             	lea    -0x1(%rcx),%esi
  4d95a4:	89 87 e8 02 00 00    	mov    %eax,0x2e8(%rdi)
  4d95aa:	8d 41 01             	lea    0x1(%rcx),%eax
  4d95ad:	0f 48 c6             	cmovs  %esi,%eax
  4d95b0:	89 87 ec 02 00 00    	mov    %eax,0x2ec(%rdi)
  4d95b6:	8b 44 24 50          	mov    0x50(%rsp),%eax
  4d95ba:	89 87 1c 03 00 00    	mov    %eax,0x31c(%rdi)
  4d95c0:	8b 44 24 54          	mov    0x54(%rsp),%eax
  4d95c4:	89 87 28 03 00 00    	mov    %eax,0x328(%rdi)
  4d95ca:	8b 44 24 58          	mov    0x58(%rsp),%eax
  4d95ce:	89 87 30 03 00 00    	mov    %eax,0x330(%rdi)
  4d95d4:	89 d0                	mov    %edx,%eax
  4d95d6:	83 e0 03             	and    $0x3,%eax
  4d95d9:	89 c1                	mov    %eax,%ecx
  4d95db:	84 c0                	test   %al,%al
  4d95dd:	75 10                	jne    4d95ef <strtol@plt+0xd3787>
  4d95df:	89 d0                	mov    %edx,%eax
  4d95e1:	c0 ea 04             	shr    $0x4,%dl
  4d95e4:	c0 e8 02             	shr    $0x2,%al
  4d95e7:	83 e2 03             	and    $0x3,%edx
  4d95ea:	83 e0 03             	and    $0x3,%eax
  4d95ed:	89 d1                	mov    %edx,%ecx
  4d95ef:	3c 01                	cmp    $0x1,%al
  4d95f1:	74 7d                	je     4d9670 <strtol@plt+0xd3808>
  4d95f3:	3c 02                	cmp    $0x2,%al
  4d95f5:	74 61                	je     4d9658 <strtol@plt+0xd37f0>
  4d95f7:	80 f9 01             	cmp    $0x1,%cl
  4d95fa:	74 68                	je     4d9664 <strtol@plt+0xd37fc>
  4d95fc:	80 f9 02             	cmp    $0x2,%cl
  4d95ff:	74 47                	je     4d9648 <strtol@plt+0xd37e0>
  4d9601:	48 8b 2d e0 e9 17 00 	mov    0x17e9e0(%rip),%rbp        # 657fe8 <strtol@plt+0x252180>
  4d9608:	48 85 ed             	test   %rbp,%rbp
  4d960b:	74 0c                	je     4d9619 <strtol@plt+0xd37b1>
  4d960d:	48 89 df             	mov    %rbx,%rdi
  4d9610:	e8 4b c4 f2 ff       	call   405a60 <pthread_mutex_lock@plt>
  4d9615:	85 c0                	test   %eax,%eax
  4d9617:	75 76                	jne    4d968f <strtol@plt+0xd3827>
  4d9619:	80 bb 04 02 00 00 00 	cmpb   $0x0,0x204(%rbx)
  4d9620:	75 5e                	jne    4d9680 <strtol@plt+0xd3818>
  4d9622:	48 85 ed             	test   %rbp,%rbp
  4d9625:	74 08                	je     4d962f <strtol@plt+0xd37c7>
  4d9627:	48 89 df             	mov    %rbx,%rdi
  4d962a:	e8 a1 be f2 ff       	call   4054d0 <pthread_mutex_unlock@plt>
  4d962f:	48 8b bb e8 11 00 00 	mov    0x11e8(%rbx),%rdi
  4d9636:	48 83 c4 08          	add    $0x8,%rsp
  4d963a:	5b                   	pop    %rbx
  4d963b:	5d                   	pop    %rbp
  4d963c:	e9 7f 97 fe ff       	jmp    4c2dc0 <strtol@plt+0xbcf58>
  4d9641:	0f 1f 80 00 00 00 00 	nopl   0x0(%rax)
  4d9648:	c6 83 05 03 00 00 01 	movb   $0x1,0x305(%rbx)
  4d964f:	eb b0                	jmp    4d9601 <strtol@plt+0xd3799>
  4d9651:	0f 1f 80 00 00 00 00 	nopl   0x0(%rax)
  4d9658:	c6 83 04 03 00 00 01 	movb   $0x1,0x304(%rbx)
  4d965f:	80 f9 01             	cmp    $0x1,%cl
  4d9662:	75 98                	jne    4d95fc <strtol@plt+0xd3794>
  4d9664:	c6 83 05 03 00 00 00 	movb   $0x0,0x305(%rbx)
  4d966b:	eb 94                	jmp    4d9601 <strtol@plt+0xd3799>
  4d966d:	0f 1f 00             	nopl   (%rax)
  4d9670:	c6 83 04 03 00 00 00 	movb   $0x0,0x304(%rbx)
  4d9677:	e9 7b ff ff ff       	jmp    4d95f7 <strtol@plt+0xd378f>
  4d967c:	0f 1f 40 00          	nopl   0x0(%rax)
  4d9680:	48 8d 7b 28          	lea    0x28(%rbx),%rdi
  4d9684:	31 d2                	xor    %edx,%edx
  4d9686:	31 f6                	xor    %esi,%esi
  4d9688:	e8 63 47 00 00       	call   4dddf0 <strtol@plt+0xd7f88>
  4d968d:	eb 93                	jmp    4d9622 <strtol@plt+0xd37ba>
  4d968f:	89 c7                	mov    %eax,%edi
  4d9691:	e8 3d 6e f4 ff       	call   4204d3 <strtol@plt+0x1a66b>
  4d9696:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  4d969d:	00 00 00 
