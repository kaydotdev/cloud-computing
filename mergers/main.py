import fire


class Merger(object):
    @staticmethod
    def hash_join(left_table, right_table):
        print(f'Hash joining {left_table} and {right_table}')

    @staticmethod
    def loop_join(left_table, right_table):
        print(f'Loop joining {left_table} and {right_table}')

    @staticmethod
    def merge_join(left_table, right_table):
        print(f'Merge joining {left_table} and {right_table}')


if __name__ == '__main__':
    fire.Fire(Merger)
