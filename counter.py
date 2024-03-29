# !py -3

import os


COUNT = 0


def dfs(path):
    dl = os.listdir(path)
    for f in dl:
        pf = path+os.sep+f
        if os.path.isdir(pf):
            dfs(pf)
        else:
            if '.g.' not in f and f.endswith("cs") and 'obj' not in pf:
                counter(pf)


def counter(csfile):
    global COUNT
    print("Counting, ", csfile)
    with open(csfile, 'r', encoding='utf-8') as f:
        COUNT += len(f.readlines())


if __name__ == "__main__":
    dfs(r"./")
    print(COUNT, "lines")
