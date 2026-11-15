
/usr/bin/nvidia-powerd:     file format elf64-x86-64


Disassembly of section .text:

00000000004dd860 <.text+0xd79f0>:
  4dd860:	41 89 f2             	mov    %esi,%r10d
  4dd863:	44 89 c8             	mov    %r9d,%eax
  4dd866:	45 85 c9             	test   %r9d,%r9d
  4dd869:	75 08                	jne    4dd873 <strtol@plt+0xd7a0b>
  4dd86b:	44 89 c0             	mov    %r8d,%eax
  4dd86e:	45 85 c0             	test   %r8d,%r8d
  4dd871:	74 05                	je     4dd878 <strtol@plt+0xd7a10>
  4dd873:	c3                   	ret
  4dd874:	0f 1f 40 00          	nopl   0x0(%rax)
  4dd878:	89 fe                	mov    %edi,%esi
  4dd87a:	03 74 24 08          	add    0x8(%rsp),%esi
  4dd87e:	41 0f 48 f0          	cmovs  %r8d,%esi
  4dd882:	85 c9                	test   %ecx,%ecx
  4dd884:	74 1a                	je     4dd8a0 <strtol@plt+0xd7a38>
  4dd886:	39 f1                	cmp    %esi,%ecx
  4dd888:	0f 47 ce             	cmova  %esi,%ecx
  4dd88b:	89 f8                	mov    %edi,%eax
  4dd88d:	ba 00 00 00 00       	mov    $0x0,%edx
  4dd892:	03 44 24 10          	add    0x10(%rsp),%eax
  4dd896:	0f 48 c2             	cmovs  %edx,%eax
  4dd899:	39 c8                	cmp    %ecx,%eax
  4dd89b:	0f 42 c1             	cmovb  %ecx,%eax
  4dd89e:	c3                   	ret
  4dd89f:	90                   	nop
  4dd8a0:	45 8d 42 01          	lea    0x1(%r10),%r8d
  4dd8a4:	41 83 f8 01          	cmp    $0x1,%r8d
  4dd8a8:	76 1e                	jbe    4dd8c8 <strtol@plt+0xd7a60>
  4dd8aa:	45 85 d2             	test   %r10d,%r10d
  4dd8ad:	41 8d 42 ff          	lea    -0x1(%r10),%eax
  4dd8b1:	41 0f 48 c0          	cmovs  %r8d,%eax
  4dd8b5:	01 f8                	add    %edi,%eax
  4dd8b7:	85 c0                	test   %eax,%eax
  4dd8b9:	7e d0                	jle    4dd88b <strtol@plt+0xd7a23>
  4dd8bb:	39 f0                	cmp    %esi,%eax
  4dd8bd:	0f 46 f0             	cmovbe %eax,%esi
  4dd8c0:	89 f1                	mov    %esi,%ecx
  4dd8c2:	eb c7                	jmp    4dd88b <strtol@plt+0xd7a23>
  4dd8c4:	0f 1f 40 00          	nopl   0x0(%rax)
  4dd8c8:	39 f2                	cmp    %esi,%edx
  4dd8ca:	0f 46 f2             	cmovbe %edx,%esi
  4dd8cd:	89 f1                	mov    %esi,%ecx
  4dd8cf:	eb ba                	jmp    4dd88b <strtol@plt+0xd7a23>
  4dd8d1:	66 2e 0f 1f 84 00 00 	cs nopw 0x0(%rax,%rax,1)
  4dd8d8:	00 00 00 
  4dd8db:	0f 1f 44 00 00       	nopl   0x0(%rax,%rax,1)
  4dd8e0:	c6 87 dc 01 00 00 00 	movb   $0x0,0x1dc(%rdi)
  4dd8e7:	c6 47 6c 00          	movb   $0x0,0x6c(%rdi)
  4dd8eb:	c3                   	ret
  4dd8ec:	0f 1f 40 00          	nopl   0x0(%rax)
