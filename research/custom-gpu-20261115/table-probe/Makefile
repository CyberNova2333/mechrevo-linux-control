obj-m += mechrevo_gpu_table_probe.o
all:
	$(MAKE) -C /lib/modules/6.8.0-149-generic/build M=$(CURDIR) CC=x86_64-linux-gnu-gcc-13 W=1 modules
clean:
	$(MAKE) -C /lib/modules/6.8.0-149-generic/build M=$(CURDIR) clean
