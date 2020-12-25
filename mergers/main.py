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
    def loop_join(left_table: list, left_index: int,
                  right_table: list, right_index: int) -> list:
        occurrences = []

        for left_cortege in left_table:
            for right_cortege in right_table:
                if left_cortege[left_index] == right_cortege[right_index]:
                    occurrences.append((left_cortege, right_cortege))

        return occurrences

    @staticmethod
    def merge_join(left_table: list, right_table: list):
        print(f'Merge joining {left_table} and {right_table}')


if __name__ == '__main__':
    fire.Fire(Merger)


