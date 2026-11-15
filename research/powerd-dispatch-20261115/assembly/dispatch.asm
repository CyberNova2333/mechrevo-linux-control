
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004c5090 <.text+0xbf220>:
  4c5090:	41 54                	push   %r12
  4c5092:	55                   	push   %rbp
  4c5093:	53                   	push   %rbx
  4c5094:	48 8b 6f 28          	mov    0x28(%rdi),%rbp
  4c5098:	4c 8b 67 30          	mov    0x30(%rdi),%r12
  4c509c:	4c 39 e5             	cmp    %r12,%rbp
  4c509f:	74 4e                	je     4c50ef <strtol@plt+0xbf287>
  4c50a1:	48 89 f3             	mov    %rsi,%rbx
  4c50a4:	0f 1f 40 00          	nopl   0x0(%rax)
  4c50a8:	48 8b 7d 00          	mov    0x0(%rbp),%rdi
  4c50ac:	f3 0f 6f 03          	movdqu (%rbx),%xmm0
  4c50b0:	48 83 ec 40          	sub    $0x40,%rsp
  4c50b4:	48 83 c5 08          	add    $0x8,%rbp
  4c50b8:	48 8b 07             	mov    (%rdi),%rax
  4c50bb:	0f 11 04 24          	movups %xmm0,(%rsp)
  4c50bf:	f3 0f 6f 4b 10       	movdqu 0x10(%rbx),%xmm1
  4c50c4:	0f 11 4c 24 10       	movups %xmm1,0x10(%rsp)
  4c50c9:	f3 0f 6f 53 20       	movdqu 0x20(%rbx),%xmm2
  4c50ce:	0f 11 54 24 20       	movups %xmm2,0x20(%rsp)
  4c50d3:	48 8b 53 30          	mov    0x30(%rbx),%rdx
  4c50d7:	48 89 54 24 30       	mov    %rdx,0x30(%rsp)
  4c50dc:	8b 53 38             	mov    0x38(%rbx),%edx
  4c50df:	89 54 24 38          	mov    %edx,0x38(%rsp)
  4c50e3:	ff 50 38             	call   *0x38(%rax)
  4c50e6:	48 83 c4 40          	add    $0x40,%rsp
  4c50ea:	49 39 ec             	cmp    %rbp,%r12
  4c50ed:	75 b9                	jne    4c50a8 <strtol@plt+0xbf240>
  4c50ef:	5b                   	pop    %rbx
  4c50f0:	5d                   	pop    %rbp
  4c50f1:	41 5c                	pop    %r12
  4c50f3:	c3                   	ret
  4c50f4:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  4c50fb:	00 00 00 
  4c50fe:	66 90                	xchg   %ax,%ax
