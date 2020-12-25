import fire


class Merger(object):
    @staticmethod
    def hash_join(left_table: list, left_index: int,
                  right_table: list, right_index: int) -> list:
        hashTable = defaultdict(list)

        for s in left_table:
            hashTable[s[left_index]].append(s)

        return [(s, r) for r in right_table for s in hashTable[r[right_index]]]

    @staticmethod
    def loop_join(left_table: list, right_table: list):
        print(f'Hash joining {left_table} and {right_table}')

    @staticmethod
    def merge_join(left_table: list, right_table: list):
        print(f'Merge joining {left_table} and {right_table}')


if __name__ == '__main__':
    fire.Fire(Merger)


