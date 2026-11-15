obj-m += mechrevo_turbo_lease.o
KDIR ?= /lib/modules/6.8.0-149-generic/build

.PHONY: all clean
all:
	$(MAKE) -C $(KDIR) M=$(CURDIR) modules
clean:
	$(MAKE) -C $(KDIR) M=$(CURDIR) clean
